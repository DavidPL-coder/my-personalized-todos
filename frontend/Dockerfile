FROM node:18.17.0
WORKDIR /usr/src/app

COPY package*.json app.js ./
COPY public/ ./public/
COPY initialization/ ./initialization/

ENV NODE_ENV=production

RUN npm install

CMD ["node", "app.js"]