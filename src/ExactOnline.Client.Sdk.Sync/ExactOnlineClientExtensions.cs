﻿using System.Reflection;
using ExactOnline.Client.Sdk.Controllers;

namespace ExactOnline.Client.Sdk.Sync;

public static class ExactOnlineClientExtensions
{
	private static readonly MethodInfo _synchronizeWithMethod = GetMethod(nameof(SynchronizeWith));
	private static readonly MethodInfo _synchronizeWithAsyncMethod = GetMethod(nameof(SynchronizeWithAsync));

	private static MethodInfo GetMethod(string methodName) =>
		typeof(ExactOnlineClientExtensions)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(m => m.Name == methodName && m.GetGenericArguments().Length == 1);

	public static SyncResult SynchronizeWith(this ExactOnlineClient client, ISyncTarget syncTarget, Type modelType, params string[] fields) =>
		InvokeMethod<SyncResult>(_synchronizeWithMethod, modelType, client, syncTarget, fields);

	public static Task<SyncResult> SynchronizeWithAsync(this ExactOnlineClient client, ISyncTarget syncTarget, Type modelType, string[] fields, Action<int, int> reportProgress = null, CancellationToken ct = default) =>
		InvokeMethod<Task<SyncResult>>(_synchronizeWithAsyncMethod, modelType, client, syncTarget, fields, reportProgress, ct);

	private static TReturn InvokeMethod<TReturn>(MethodInfo method, Type modelType, params object[] parameters) =>
		// here we call SynchronizeWith or SynchronizeWithAsync<TModel>(query, syncTarget, fields) with the right TModel for modelType
		(TReturn)method
			.MakeGenericMethod(modelType)
			.Invoke(null, parameters);

	public static SyncResult SynchronizeWith<TModel>(this ExactOnlineClient client, ISyncTarget syncTarget, params string[] fields)
		where TModel : class =>
		client.For<TModel>().Select(fields).SynchronizeWith(syncTarget, client, fields);

	public static Task<SyncResult> SynchronizeWithAsync<TModel>(this ExactOnlineClient client, ISyncTarget syncTarget, string[] fields, Action<int, int> reportProgress = null, CancellationToken ct = default)
		where TModel : class =>
		client.For<TModel>().Select(fields).SynchronizeWithAsync(syncTarget, client, fields, reportProgress, ct);
}
