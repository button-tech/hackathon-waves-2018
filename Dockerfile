FROM nginx:1.15.7-alpine

COPY frontend /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

RUN apk update && apk add git

EXPOSE 80