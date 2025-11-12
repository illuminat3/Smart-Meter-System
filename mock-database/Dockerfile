FROM node:lts-alpine3.22

RUN npm install -g json-server

WORKDIR /src

COPY db.json ./

EXPOSE 3000

CMD ["json-server", "db.json"]
