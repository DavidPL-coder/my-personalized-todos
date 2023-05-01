FROM node:14
WORKDIR /usr/src/app

COPY package*.json app.js ./
COPY public/ ./public/
COPY initialization/ ./initialization/

ENV NODE_ENV=production

RUN npm install

# EXPOSE 443

CMD ["node", "app.js"]