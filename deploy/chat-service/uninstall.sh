#!/bin/bash

sfctl application delete --application-id chat-service
sfctl application unprovision --application-type-name chat-serviceType --application-type-version 1.0.0
sfctl store delete --content-path chat-service
