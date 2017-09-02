#!/usr/bin/env bash

rsync --exclude='.git' -r ./ $1:/srv/Nadeko
ssh $1 'cd /srv/Nadeko && sudo docker-compose build --pull'
echo '!!!! BOT IS GOING DOWN !!!!'
ssh $1 'cd /srv/Nadeko && sudo docker-compose down && sudo docker-compose up -d'
echo '--- WAITING FOR BOT TO COME BACK (this can take a minute or two) ---'
time ssh $1 'sudo docker logs -f nadeko_bot_1 | grep -qF "Shard 0 ready."'
echo '!!!! BOT IS UP !!!!'
