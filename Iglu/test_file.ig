let success = false;

let test = 0;
let answer = 10;

for(let i = 0; i < 10; i = i + 1)
{
	test = test + 1;
}

success = test == answer;

print success ? "pass" : "fail";