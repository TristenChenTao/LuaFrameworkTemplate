﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class HTTPClientWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(HTTPClient), typeof(System.Object));
		L.RegFunction("Request", Request);
		L.RegFunction("LoadWebImage", LoadWebImage);
		L.RegFunction("New", _CreateHTTPClient);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateHTTPClient(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				HTTPClient obj = new HTTPClient();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: HTTPClient.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Request(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				UnityEngine.WWWForm arg2 = (UnityEngine.WWWForm)ToLua.CheckObject<UnityEngine.WWWForm>(L, 3);
				HTTPClient.Request(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 4)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				UnityEngine.WWWForm arg2 = (UnityEngine.WWWForm)ToLua.CheckObject<UnityEngine.WWWForm>(L, 3);
				LuaFunction arg3 = ToLua.CheckLuaFunction(L, 4);
				HTTPClient.Request(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: HTTPClient.Request");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LoadWebImage(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				HTTPClient.LoadWebImage(arg0);
				return 0;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				LuaFunction arg1 = ToLua.CheckLuaFunction(L, 2);
				HTTPClient.LoadWebImage(arg0, arg1);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: HTTPClient.LoadWebImage");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
