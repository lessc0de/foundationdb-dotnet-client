﻿#region BSD Licence
/* Copyright (c) 2013, Doxense SARL
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
	* Redistributions of source code must retain the above copyright
	  notice, this list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright
	  notice, this list of conditions and the following disclaimer in the
	  documentation and/or other materials provided with the distribution.
	* Neither the name of Doxense nor the
	  names of its contributors may be used to endorse or promote products
	  derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

namespace FoundationDB.Filters.Logging
{
	using FoundationDB.Client;
	using JetBrains.Annotations;
	using System;

	/// <summary>Set of extension methods that add logging support on transactions</summary>
	public static class FdbLoggingExtensions
	{

		[NotNull]
		public static FdbLoggedDatabase Logged([NotNull] this IFdbDatabase database, [NotNull] Action<FdbLoggedTransaction> handler, FdbLoggingOptions options = FdbLoggingOptions.Default)
		{
			if (database == null) throw new ArgumentNullException("database");
			if (handler == null) throw new ArgumentNullException("handler");

			// prevent multiple logging
			database = WithoutLogging(database);

			return new FdbLoggedDatabase(database, false, false, handler, options);
		}

		/// <summary>Strip the logging behaviour of this database. Use this for boilerplate or test code that would pollute the logs otherwise.</summary>
		/// <param name="database">Database instance (that may or may not be logged)</param>
		/// <returns>Either <paramref name="database"/> itself if it is not logged, or the inner database if it was.</returns>
		public static IFdbDatabase WithoutLogging(this IFdbDatabase database)
		{
			var logged = database as FdbLoggedDatabase;
			if (logged != null) return logged.GetInnerDatabase();

			return database;
		}

		internal static FdbLoggedTransaction GetLogger(IFdbReadOnlyTransaction trans)
		{
			//TODO: the logged transaction could also be wrapped in other filters.
			// => we need a recursive "FindFilter<TFilter>" method that would unwrap the filter onion looking for a specific one...

			return trans as FdbLoggedTransaction;
		}

		/// <summary>Annotate a logged transaction</summary>
		public static void Annotate(this IFdbReadOnlyTransaction trans, string message)
		{
			var logged = GetLogger(trans);
			if (logged != null)
			{
				logged.Log.AddOperation(new FdbTransactionLog.LogCommand(message), countAsOperation: false);
			}
		}

		/// <summary>Annotate a logged transaction</summary>
		[StringFormatMethod("format")]
		public static void Annotate(this IFdbReadOnlyTransaction trans, string format, object arg0)
		{
			var logged = GetLogger(trans);
			if (logged != null) logged.Log.AddOperation(new FdbTransactionLog.LogCommand(String.Format(format, arg0)), countAsOperation: false);
		}

		/// <summary>Annotate a logged transaction</summary>
		[StringFormatMethod("format")]
		public static void Annotate(this IFdbReadOnlyTransaction trans, string format, object arg0, object arg1)
		{
			var logged = GetLogger(trans);
			if (logged != null) logged.Log.AddOperation(new FdbTransactionLog.LogCommand(String.Format(format, arg0, arg1)), countAsOperation: false);
		}

		/// <summary>Annotate a logged transaction</summary>
		[StringFormatMethod("format")]
		public static void Annotate(this IFdbReadOnlyTransaction trans, string format, object arg0, object arg1, object arg2)
		{
			var logged = GetLogger(trans);
			if (logged != null) logged.Log.AddOperation(new FdbTransactionLog.LogCommand(String.Format(format, arg0, arg1, arg2)), countAsOperation: false);
		}

		/// <summary>Annotate a logged transaction</summary>
		[StringFormatMethod("format")]
		public static void Annotate(this IFdbReadOnlyTransaction trans, string format, params object[] args)
		{
			var logged = GetLogger(trans);
			if (logged != null) logged.Log.AddOperation(new FdbTransactionLog.LogCommand(String.Format(format, args)), countAsOperation: false);
		}

	}

}
