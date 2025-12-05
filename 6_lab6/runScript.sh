#!/bin/zsh

if [[ $# -eq 0 ]]; then
    echo "Usage: $0 <source_file.c>"
    exit 1
fi

src_file=$1

if [[ ! -f $src_file ]]; then
    echo "Error: file '$src_file' not found."
    exit 1
fi

base_name="${src_file:r}"
echo "Compiling $src_file..."
gcc -lrt -lm -lpthread -Wall "$src_file"  -o "$base_name"

# Check for success
if [[ $? -eq 0 ]]; then
    echo "Compilation successful!"
else
    echo "Compilation failed."
    exit 1
fi
