# ── Stage 1: Build ───────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# Copy lockfiles first — npm ci is cached until these change
COPY package.json package-lock.json .
RUN npm ci

# Copy source and build for production
COPY . .
RUN npx ng build --configuration production

# ── Stage 2: Serve ───────────────────────────────────────────────
FROM nginx:1.27-alpine AS runtime

# Remove default Nginx page
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular build output
COPY --from=build /app/dist/weather-ui/browser /usr/share/nginx/html

# Optional: custom nginx config (see tip below)
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
