﻿
using System;
using System.IO;
using System.Diagnostics;

namespace Sider
{
  public partial class RedisClient
  {
    public bool Ping()
    {
      _writer.WriteLine("PING");

      return _reader.ReadTypeChar() == ResponseType.SingleLine &&
        _reader.ReadStatusLine() == "PONG";
    }

    public int Del(params string[] keys)
    {
      _writer.WriteLine("DEL {0}".F(string.Join(" ", keys)));

      var success = _reader.ReadTypeChar() == ResponseType.Integer;

      Assert.IsTrue(success, () => new ResponseException(
        "Issused a DEL but didn't receive an expected integer reply."));

      return _reader.ReadNumberLine();
    }


    public bool Set(string key, string value)
    {
      return SetRaw(key, encodeStr(value));
    }

    public bool SetRaw(string key, byte[] raw)
    {
      _writer.WriteLine("SET {0} {1}".F(key, raw.Length));
      _writer.WriteBulk(raw);

      return _reader.ReadTypeChar() == ResponseType.SingleLine &&
        _reader.ReadStatusLine() == "OK";
    }

    public bool SetFrom(string key, Stream source, int count)
    {
      _writer.WriteLine("SET {0} {1}".F(key, count));
      _writer.WriteBulkFrom(source, count);

      return _reader.ReadTypeChar() == ResponseType.SingleLine &&
        _reader.ReadStatusLine() == "OK";
    }


    public string Get(string key)
    {
      return decodeStr(GetRaw(key));
    }

    public byte[] GetRaw(string key)
    {
      _writer.WriteLine("GET {0}".F(key));

      var success = _reader.ReadTypeChar() == ResponseType.Bulk;

      Assert.IsTrue(success, () => new ResponseException(
        "Issued a GET but didn't receive an expected bulk reply."));

      var length = _reader.ReadNumberLine();

      return length > -1 ?
        _reader.ReadBulk(length) :
        new byte[] { };
    }

    public int GetTo(string key, Stream target)
    {
      _writer.WriteLine("GET {0}".F(key));

      var success = _reader.ReadTypeChar() == ResponseType.Bulk;

      Assert.IsTrue(success, () => new ResponseException(
        "Issued a GET but didn't receive an expected bulk reply."));

      var length = _reader.ReadNumberLine();
      if (length > -1)
        _reader.ReadBulkTo(target, length);

      return length;
    }


    public long Incr(string key)
    {
      _writer.WriteLine("INCR {0}".F(key));

      var success = _reader.ReadTypeChar() == ResponseType.Integer;

      Assert.IsTrue(success, () => new ResponseException(
        "Issued a GET but didn't receive an expected integer reply."));

      return _reader.ReadNumberLine64();
    }


  }
}
