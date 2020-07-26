#pragma once

#include "common.h"
#include "chunk.h"

#define FREE(type, pointer) reallocate(pointer, sizeof(type), 0)

#define GROW_CAPACITY(capacity) \
	((capacity < 8) ? 8 : (capacity * 2));
	
#define ALLOCATE(type, size) \
	(type*)reallocate(NULL, 0, sizeof(type) * size);

#define GROW_ARRAY(type, pointer, oldCapacity, newCapacity) \
	((type*) reallocate(pointer, sizeof(type) * oldCapacity, sizeof(type) * newCapacity))

#define FREE_ARRAY(type, pointer, capacity) \
	(reallocate(pointer, sizeof(type) * capacity, 0))

void* reallocate(void* pointer, size_t oldSize, size_t newSize);
void freeObjects();