FROM microsoft/dotnet:2.0-sdk
MAINTAINER proegssilb <proegssilb@gmail.com>

WORKDIR /opt/

#Install required software
RUN echo "deb http://ftp.utexas.edu/dotdeb stable all" > /etc/apt/sources.list.d/dotdeb.list \
	&& echo "deb-src http://ftp.utexas.edu/dotdeb stable all" >> /etc/apt/sources.list.d/dotdeb.list \
	&& wget -O /tmp/dotdeb.gpg https://www.dotdeb.org/dotdeb.gpg \
	&& apt-key add /tmp/dotdeb.gpg \
	&& touch /etc/inittab \
	&& apt-get update \
	&& DEBIAN_FRONTEND=noninteractive apt-get install -y git libopus0 opus-tools libopus-dev libsodium-dev ffmpeg redis-server runit \
	&& rm /tmp/dotdeb.gpg \
	&& mkdir -p /etc/service/redis/ \
	&& echo "#!/bin/sh\n\nredis-server /etc/redis/redis.conf" > /etc/service/redis/run \
	&& chmod 755 /etc/service/redis/run

#Download and install stable version of Nadeko
RUN curl -L https://github.com/Kwoth/NadekoBot-BashScript/raw/master/nadeko_installer_latest.sh | sh \
	&& mkdir -p /etc/service/nadeko \
	&& curl -L https://github.com/Kwoth/NadekoBot-BashScript/raw/master/nadeko_autorestart.sh | sed "/if hash.*/ { N; s_if hash dotnet 2>/dev/null\n_cd /opt/\n&_ }" > /etc/service/nadeko/run \
	&& chmod 755 /etc/service/nadeko/run

#Apply pogogr customizations
ADD . /opt/NadekoBotNew/
RUN mv /opt/NadekoBot /opt/NadekoBotOld \
	&& mv /opt/NadekoBotNew /opt/NadekoBot \
	&& cd NadekoBot \
	&& dotnet restore \
	&& cp Procfile ../ \
	&& cd /opt

CMD ["/usr/bin/runsvdir", "/etc/service"]
