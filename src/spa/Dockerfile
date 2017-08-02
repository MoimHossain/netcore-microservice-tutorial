FROM node:boron

COPY . /app
WORKDIR /app

RUN npm install
RUN npm install webpack-dev-server -g
RUN npm rebuild node-sass
CMD [ "webpack" ] 

EXPOSE 3000
CMD [ "npm", "start" ]