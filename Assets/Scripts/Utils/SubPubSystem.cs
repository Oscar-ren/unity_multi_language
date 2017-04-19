using UnityEngine;
using System;
using System.Collections.Generic;

public class SubPubSystem
{
	public Dictionary<string, Delegate> records = new Dictionary<string, Delegate>();

	public void Subscribe(string name, Action method) { _Subscribe(name, method); }
	public void Subscribe<T0>(string name, Action<T0> method) { _Subscribe(name, method); }
	public void Subscribe<T0, T1>(string name, Action<T0, T1> method) { _Subscribe(name, method); }
	public void Subscribe<T0, T1, T2>(string name, Action<T0, T1, T2> method) { _Subscribe(name, method); }
	public void Subscribe<T0, T1, T2, T3>(string name, Action<T0, T1, T2, T3> method) { _Subscribe(name, method); }

	public void UnSubscribe(string name, Action method) { _UnSubscribe(name, method); }
	public void UnSubscribe<T0>(string name, Action<T0> method) { _UnSubscribe(name, method); }
	public void UnSubscribe<T0, T1>(string name, Action<T0, T1> method) { _UnSubscribe(name, method); }
	public void UnSubscribe<T0, T1, T2>(string name, Action<T0, T1, T2> method) { _UnSubscribe(name, method); }
	public void UnSubscribe<T0, T1, T2, T3>(string name, Action<T0, T1, T2, T3> method) { _UnSubscribe(name, method); }

	public void _Subscribe(string name, Delegate method)
	{
		Delegate d;
		if (records.TryGetValue(name, out d))
		{
			d = Delegate.Combine(d, method);
			records[name] = d;
		}
		else
		{
			records.Add(name, method);
		}
	}

	public void _UnSubscribe(string name, Delegate method)
	{
		Delegate d;
		if (records.TryGetValue(name, out d))
		{
			d = Delegate.Remove(d, method);
			records[name] = d; 
		}
	}

	public void Publish(string name, params object[] args)
	{
		try
		{
			Delegate d;
			if (records.TryGetValue(name, out d))
			{
				if (d != null)
				{
					d.DynamicInvoke(args);
				}             
			}
			else
			{
				records.Remove(name);
			}
		}
		catch(Exception ex)
		{
			Debug.LogError(ex.Message); 
		}
	}
}
