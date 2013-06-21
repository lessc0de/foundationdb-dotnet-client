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
	* Neither the name of the <organization> nor the
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

using FoundationDb.Client;
using FoundationDb.Client.Converters;
using FoundationDb.Client.Utils;
using System;
using System.Collections.Generic;

namespace FoundationDb.Layers.Tuples
{

	public sealed class FdbMemoizedTuple : IFdbTuple
	{
		internal readonly object[] Items;

		internal readonly Slice Packed;

		internal FdbMemoizedTuple(object[] items, Slice packed)
		{
			Contract.Requires(items != null);
			Contract.Requires(packed.HasValue);

			this.Items = items;
			this.Packed = packed;
		}

		public int PackedSize
		{
			get { return this.Packed.Count; }
		}

		public void PackTo(FdbBufferWriter writer)
		{
			if (this.Packed.Count > 0)
			{
				writer.WriteBytes(this.Packed);
			}
		}

		public Slice ToSlice()
		{
			return this.Packed;
		}

		public int Count
		{
			get { return this.Items.Length; }
		}

		public object this[int index]
		{
			get { return this.Items[FdbTuple.MapIndex(index, this.Items.Length)]; }
		}

		public IFdbTuple this[int? from, int? to]
		{
			get { return FdbTuple.Splice(this, from, to); }
		}

		public R Get<R>(int index)
		{
			return FdbConverters.ConvertBoxed<R>(this[index]);
		}

		IFdbTuple IFdbTuple.Append<T>(T value)
		{
			return this.Append<T>(value);
		}

		public FdbLinkedTuple<T> Append<T>(T value)
		{
			return new FdbLinkedTuple<T>(this, value);
		}

		public void CopyTo(object[] array, int offset)
		{
			Array.Copy(this.Items, 0, array, offset, this.Items.Length);
		}

		public IEnumerator<object> GetEnumerator()
		{
			return ((IList<object>)this.Items).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			return FdbTuple.ToString(this.Items, 0, this.Items.Length);
		}

		public bool Equals(IFdbTuple other)
		{
			//TODO!
			return object.ReferenceEquals(this, other);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as IFdbTuple);
		}
	}
}