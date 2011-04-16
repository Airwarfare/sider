﻿
using System;
using System.Text;

namespace Sider
{
  public sealed class RedisSettings
  {
    public const string DefaultHost = "localhost";
    public const int DefaultPort = 6379;


    private static RedisSettings _default = new RedisSettings();

    public static RedisSettings Default { get { return _default; } }


    public string Host { get; private set; }
    public int Port { get; private set; }

    public bool ReconnectOnIdle { get; private set; }
    public bool ReissueWriteOnReconnect { get; private set; }
    public bool ReissueReadOnReconnect { get; private set; }

    public int ReadBufferSize { get; private set; }
    public int WriteBufferSize { get; private set; }
    public int StringBufferSize { get; private set; }
    public int SerializationBufferSize { get; private set; }

    public Encoding KeyEncoding { get; private set; }
    public Encoding ValueEncoding { get; private set; }

    public ISerializer SerializerOverride { get; private set; }

    private RedisSettings()
    {
      // set defaults
      Host = "localhost";
      Port = 6379;

      ReconnectOnIdle = true;
      ReissueWriteOnReconnect = true;
      ReissueReadOnReconnect = true;

      // TODO: Use buffer pools? with growable buffers?
      ReadBufferSize = 4096;
      WriteBufferSize = 4096;
      StringBufferSize = 256;
      SerializationBufferSize = 2048;

      KeyEncoding = Encoding.ASCII;
      ValueEncoding = Encoding.UTF8;

      SerializerOverride = null;
    }

    public static Builder New() { return new Builder(); }
    public Builder CopyNew() { return new Builder(this); }


    public sealed class Builder
    {
      private RedisSettings _settings;

      public Builder() { _settings = new RedisSettings(); }
      protected internal Builder(RedisSettings instance) { _settings = instance; }

      public static implicit operator RedisSettings(Builder b)
      {
        return b._settings;
      }


      public Builder Host(string host)
      {
        SAssert.ArgumentSatisfy(() => host, v => !string.IsNullOrEmpty(v),
          "Host cannot be null or empty.");

        _settings.Host = host;
        return this;
      }

      public Builder Port(int port)
      {
        SAssert.ArgumentBetween(() => port, 1, 65535);

        _settings.Port = port;
        return this;
      }

      public Builder ReadBufferSize(int bufferSize)
      {
        SAssert.ArgumentPositive(() => bufferSize);

        _settings.ReadBufferSize = bufferSize;
        return this;
      }

      public Builder WriteBufferSize(int bufferSize)
      {
        SAssert.ArgumentPositive(() => bufferSize);

        _settings.WriteBufferSize = bufferSize;
        return this;
      }

      public Builder StringBufferSize(int bufferSize)
      {
        SAssert.ArgumentPositive(() => bufferSize);

        _settings.StringBufferSize = bufferSize;
        return this;
      }

      public Builder SerializationBufferSize(int bufferSize)
      {
        SAssert.ArgumentPositive(() => bufferSize);

        _settings.SerializationBufferSize = bufferSize;
        return this;
      }

      public Builder BufferSize(int read, int write)
      {
        return this
          .ReadBufferSize(read)
          .WriteBufferSize(write);
      }

      public Builder BufferSize(int read, int write, int str, int serialization)
      {
        return this
          .ReadBufferSize(read)
          .WriteBufferSize(write)
          .StringBufferSize(str)
          .SerializationBufferSize(serialization);
      }

      public Builder ReconnectOnIdle(bool reconnectOnIdle)
      {
        _settings.ReconnectOnIdle = reconnectOnIdle;
        return this;
      }

      public Builder ReissueWriteOnReconnect(bool reissueWriteOnReconnect)
      {
        SAssert.IsTrue(_settings.ReconnectOnIdle, () =>
          new ArgumentException("ReissueWriteOnReconnect requires ReconnectOnIdle"));

        _settings.ReissueWriteOnReconnect = reissueWriteOnReconnect;
        return this;
      }

      public Builder ReissueReadOnReconnect(bool reissueReadOnReconnect)
      {
        SAssert.IsTrue(_settings.ReconnectOnIdle, () =>
          new ArgumentException("ReissueReadOnReconnect requires ReconnectOnIdle"));

        _settings.ReissueReadOnReconnect = reissueReadOnReconnect;
        return this;
      }

      public Builder ValueEncoding(Encoding encoding)
      {
        SAssert.ArgumentNotNull(() => encoding);

        _settings.ValueEncoding = encoding;
        return this;
      }

      public Builder KeyEncoding(Encoding encoding)
      {
        SAssert.ArgumentNotNull(() => encoding);

        _settings.KeyEncoding = encoding;
        return this;
      }

      public Builder OverrideSerializer(ISerializer serializer)
      {
        // can be null
        _settings.SerializerOverride = serializer;
        return this;
      }
    }
  }
}
