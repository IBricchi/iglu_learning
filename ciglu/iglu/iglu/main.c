#include "common.h"
#include "chunk.h"
#include "debug.h"
#include "vm.h"

int main(int argc, char** argv)
{
	initVM();

	Chunk chunk;
	initChunk(&chunk);

	uint8_t constant = addConstant(&chunk, 1.2);
	writeChunk(&chunk, OP_CONSTANT, 1);
	writeChunk(&chunk, constant, 1);
	writeChunk(&chunk, OP_NEGATE, 1);

	writeChunk(&chunk, OP_RETURN, 1);

	//dissasembleChunk(&chunk, "test chunk");
	interpret(&chunk);

	freeVM();
	freeChunk(&chunk);

	return 0;
}