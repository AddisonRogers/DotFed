#FROM registry.access.redhat.com/ubi9/ubi:9.2-696
FROM node:20-alpine3.17

WORKDIR /

COPY package*.json ./

RUN node install

COPY . .

ENV PORT = 8080

EXPOSE 8080

CMD [ "node", "start" ]

# Something like this