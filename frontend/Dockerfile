FROM node:22-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

ENV VITE_PREVIEW_PORT=4173

EXPOSE 4173

CMD ["sh", "-c", "npm run build && npm run preview -- --port ${VITE_PREVIEW_PORT} --host 0.0.0.0"]
