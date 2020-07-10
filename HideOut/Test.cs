using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.HideOut
{
	public class Test : MonoBehaviour
	{
		private List<TestMachine> _testList;
		private readonly Predicate<TestMachine> _dance = m => m.Power == 100;

		private Func<int, int, int> _add = (a, b) => a + b;
		private Func<int, int, int> _sub = (a, b) => a - b;
		private Func<int, int, int> _func;

		private void Start()
		{
			_testList = new List<TestMachine>()
			{
				new TestMachine() {Name = "Sex Machine", Power = 100},
				new TestMachine() {Name = "Dance Machine", Power = 120},
			};

			print(_testList.Find(_dance).Name);
			print(_testList.Find(m => m.Power == 120).Name);

			_func += _add;
			_add += _sub;
		}
	}

	public class TestMachine
	{
		public string Name = "Default";
		public int Power = 0;
	}
}
