#!/bin/bash -e

# This should be run from the root of the repo.

projects=(chat-service)

docker build --target build-env -t monster-blast/build-env:latest -f ./deploy/Dockerfile ./source

for project in ${projects[@]}; do
    docker build --target ${project} -t lemvic/${project}:latest -f ./deploy/Dockerfile ./source
done
