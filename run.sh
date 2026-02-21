#! /bin/bash

PAINT_APP_DIR="/Paint-The-Shed"

echo "Running app..."

cd Paint-The-Shed && echo "In directory ${PAINT_APP_DIR}"

dotnet build
dotnet run

echo "done"
