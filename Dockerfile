FROM microsoft/dotnet:1.0.5-sdk
MAINTAINER Poag <poag@gany.net>

WORKDIR /opt/

#Install required software
RUN echo "deb http://www.deb-multimedia.org jessie main non-free" | tee /etc/apt/sources.list.d/debian-backports.list \
	&& apt-get update \
	&& apt-get install -y --force-yes deb-multimedia-keyring \
	&& apt-get update \
	&& apt-get install -y git libopus0 opus-tools libopus-dev libsodium-dev ffmpeg

#Download and install stable version of Nadeko
RUN curl -L https://github.com/Kwoth/NadekoBot-BashScript/raw/master/nadeko_installer_latest.sh | sh \
	&& curl -L https://github.com/Kwoth/NadekoBot-BashScript/raw/master/nadeko_autorestart.sh > nadeko.sh \
	&& chmod 755 nadeko.sh

#Apply pogogr customizations
VOLUME ["/opt/nadekoData"]
COPY ./* /opt/NadekoBotNew/
RUN mv /opt/NadekoBot /opt/NadekoBotOld \
	&& mv /opt/NadekoBotNew /opt/NadekoBot \
	&& cd NadekoBot \
	&& ls \
	&& dotnet restore \
	&& cd src/NadekoBot \
	&& dotnet build -c Release \
	&& cd /opt \
	&& touch /opt/nadekoData/NadekoBot.db \
	&& touch /opt/nadekoData/credentials.json \
	&& ln -sf /opt/nadekoData/NadekoBot.db /opt/NadekoBot/src/NadekoBot/data/NadekoBot.db \
	&& ln -sf /opt/nadekoData/credentials.json /opt/NadekoBot/src/NadekoBot/credentials.json

CMD ["/opt/nadeko.sh"]
