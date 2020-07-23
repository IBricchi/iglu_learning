#pragma once

typedef struct {
	const char* start;
	const char* current;
	int line;
} Scanner;

void initScanner(const char* source);