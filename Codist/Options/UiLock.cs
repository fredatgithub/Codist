﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Codist.Options
{
	sealed class UiLock
	{
		int _locked;

		public bool IsLocked => _locked != 0;
		public Action CommonAction { get; set; }
		public bool Lock() {
			return Interlocked.CompareExchange(ref _locked, 1, 0) == 0;
		}
		public bool Unlock() {
			return Interlocked.CompareExchange(ref _locked, 0, 1) == 1;
		}
		public EventHandler HandleEvent (Action action) {
			return (sender, args) => {
				if (Lock()) {
					try {
						CommonAction();
						action();
					}
					finally {
						Unlock();
					}
				}
			};
		}
		public bool DoWithLock(Action action) {
			if (Lock()) {
				try {
					action();
				}
				finally {
					Unlock();
				}
				return true;
			}
			return false;
		}
	}
}
