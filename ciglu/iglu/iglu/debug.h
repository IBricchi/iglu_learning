#pragma once

#include "chunk.h"

void dissasembleChunk(Chunk* chunk, const char* name);
int dissasembleInstruction(Chunk* chunk, int offset);