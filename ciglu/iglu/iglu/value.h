#pragma once

#include "common.h"

typedef double Value;
void printValue(Value value);

typedef struct {
	int count;
	int capacity;
	Value* values;
} ValueArray;

void initValueArray(ValueArray* array);
void freeValueArray(ValueArray* array);
void writeValueArray(ValueArray* array, Value value);
