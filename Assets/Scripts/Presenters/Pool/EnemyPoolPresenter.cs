using System;
using System.Collections.Generic;
using EcsEnemy;
using UnityEngine;
using Views;

namespace Presenters
{
	public class EnemyPoolPresenter : IEnemyPool, IDisposable
	{
		private readonly IEnemyPoolView _view;
		private readonly Stack<CharacterEyesView> _available = new();
		private readonly HashSet<CharacterEyesView> _rented = new();
		private readonly HashSet<CharacterEyesView> _all = new();

		private bool _disposed;

		public EnemyPoolPresenter(IEnemyPoolView view)
		{
			_view = view ?? throw new ArgumentNullException(nameof(view));
		}

		public CharacterEyesView Rent()
		{
			ThrowIfDisposed();

			CharacterEyesView enemy = null;
			while (_available.Count > 0 && enemy == null)
			{
				enemy = _available.Pop();

				if (enemy == null)
					_all.Remove(enemy);
			}

			if (enemy == null)
			{
				enemy = _view.CreateInstance();

				if (enemy == null)
					throw new InvalidOperationException("Pool view returned a null instance.");

				_all.Add(enemy);
			}

			_rented.Add(enemy);
			return enemy;
		}

		public void Prepare(CharacterEyesView view, Vector3 position)
		{
			RequireRented(view);
			_view.Prepare(view, position);
		}

		public void Activate(CharacterEyesView view)
		{
			RequireRented(view);
			_view.Activate(view);
		}

		public void Return(CharacterEyesView view)
		{
			RequireRented(view);
			_view.Deactivate(view);
			_rented.Remove(view);
			_available.Push(view);
		}

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;

			foreach (CharacterEyesView view in _all)
			{
				if (view != null)
					_view.DestroyInstance(view);
			}

			_available.Clear();
			_rented.Clear();
			_all.Clear();
		}

		private void RequireRented(CharacterEyesView view)
		{
			ThrowIfDisposed();

			if (view == null || !_rented.Contains(view))
				throw new InvalidOperationException("Instance is not rented from this pool or was already returned.");
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(EnemyPoolPresenter));
		}
	}
}
