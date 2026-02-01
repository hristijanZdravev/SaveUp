FROM node:18

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Set working directory
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy the app
COPY . .

RUN ng analytics disable

# Expose dev server port
EXPOSE 4200

CMD ["ng", "serve", "--host", "0.0.0.0"]
