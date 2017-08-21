#!/usr/bin/env bash

scp -r ./ $1:/srv/Nadeko
ssh $1 'cd /srv/Nadeko && sudo docker-compose -f docker-compose.yml up -d'
