fun a()
{
	let _1 = "test1";
	fun b()
	{
		let _2 = "test2";
		fun c()
		{
			let _3 = "test3";
			return _1 + _2 + _3;
		}
		return c;
	}
	return b;
}

print a()()();