# .NET Core Microservice Tutorial

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

This repository contains a legacy .NET Core 1.0 microservice tutorial with React/Redux frontend. Components use Docker containers and are designed to demonstrate microservice architecture patterns with nginx reverse proxy routing.

## Working Effectively

### Critical Setup Requirements
- **Node.js 12.x**: REQUIRED for SPA. Modern Node.js versions will fail due to node-sass compatibility.
- **.NET Core 1.0 SDK**: Legacy SDK required. Modern .NET Core/5+ will NOT work with project.json format.
- **Docker**: Required for containerized builds and deployment.

### Bootstrap and Build Process

**NEVER CANCEL builds or long-running commands. Set timeouts of 60+ minutes for builds.**

#### Install .NET Core 1.0 SDK (Required for API)
```bash
# .NET Core 1.0 SDK is not available on modern systems
# Use Docker approach or legacy Ubuntu 16.04/CentOS 7 systems
# CANNOT build API on modern systems without Docker
```

#### SPA (React/Redux Frontend) Setup
```bash
cd src/spa

# Install Node.js 12.x using NVM (REQUIRED - newer versions fail)
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
nvm install 12.22.12
nvm use 12.22.12

# Install dependencies (MUST use --ignore-scripts due to node-sass issues)
npm install --ignore-scripts

# Run tests - takes 1-2 minutes. NEVER CANCEL.
npm test

# Run webpack build - takes 2-3 minutes. NEVER CANCEL. 
# Note: Sass compilation will show errors but JS bundle builds successfully
./node_modules/.bin/webpack

# Start development server - takes 2-3 minutes to compile. NEVER CANCEL.
# Server runs on http://localhost:3000 but has Sass compilation warnings
npm start
```

#### Docker Builds
```bash
# SPA Docker build - takes 10-15 minutes. NEVER CANCEL. Set timeout to 30+ minutes.
cd src/spa
docker build -t spa-ui .

# API Docker build - requires pre-built artifacts
cd src/api
# Note: Dockerfile expects ./bin/Release/eisen-pub/ directory with compiled API
docker build -t eisen-api .

# HA-Proxy build - takes 1-2 minutes
cd src/ha-proxy  
docker build -t app-router .
```

#### Run Containers (from README)
```bash
# Start UI container
docker run -d -p 3000:3000 spa-ui

# Start API container  
docker run -d -p 5000:5000 eisen-api

# Start Router
docker run -d -p 8086:80 app-router
```

## Validation

### Testing Requirements
- **SPA Tests**: Always run `npm test` after making changes to SPA. Tests take 1-2 minutes.
- **API Testing**: Cannot unit test API without .NET Core 1.0 SDK. Use Docker for integration testing.
- **Manual Validation**: Always test complete user scenarios after changes.

### Manual Testing Process
After making changes to the SPA, always validate by:
1. Starting the dev server: `npm start` (takes 2-3 minutes)
2. Navigate to http://localhost:3000 in browser
3. Verify the React application loads (title shows "FST Demo app")
4. Test core functionality even though styling may be missing due to Sass issues

### Screenshot Evidence
The SPA successfully runs despite CSS compilation warnings:
![SPA Running](https://github.com/user-attachments/assets/bee6264c-eac2-4cc7-84a1-807b55672424)

### Known Issues and Workarounds
- **node-sass compatibility**: Use Node.js 12.x and `npm install --ignore-scripts`
- **Sass compilation errors**: Expected due to binary incompatibility. JS builds successfully.
- **CSS styling missing**: Sass compilation fails but application functionality works.
- **.NET Core 1.0**: Cannot build on modern systems. Use Docker or legacy OS.
- **Build timeouts**: Builds take 10-30 minutes. Set appropriate timeouts, NEVER CANCEL.
- **CDN resources blocked**: External CDN resources may be blocked but application still functions.

## Project Structure

### Key Components
```
src/
├── api/           # .NET Core 1.0 Web API (legacy project.json format)
├── spa/           # React/Redux frontend with Webpack
└── ha-proxy/      # nginx reverse proxy configuration
```

### API Service (`src/api/`)
- **Framework**: .NET Core 1.0 with legacy project.json
- **Dependencies**: ASP.NET Core MVC 1.0.1, Kestrel server
- **Build**: Requires .NET Core 1.0 SDK (not available on modern systems)
- **Port**: 5000
- **Docker**: Uses microsoft/aspnetcore:1.0.1 base image

### SPA Application (`src/spa/`)
- **Framework**: React 15.1.0, Redux 3.5.2, Webpack 1.13.1
- **Build Tool**: Webpack with Babel, Sass loader
- **Development**: webpack-dev-server on port 3000
- **Testing**: Mocha test runner with Chai assertions
- **Known Issues**: node-sass binary incompatibility, requires Node.js 12.x

### HA-Proxy (`src/ha-proxy/`)
- **Purpose**: nginx reverse proxy for subdomain routing
- **Configuration**: Routes api.example.com and ui.example.com
- **Testing**: Requires hosts file modification for local testing

## Common Commands and Expected Times

### Verified Working Commands
```bash
# SPA development (VALIDATED - exact times measured)
cd src/spa && npm test                    # 4 seconds (very fast)
cd src/spa && npm start                   # 2 seconds to start, loads on http://localhost:3000

# Docker builds (NEVER CANCEL - set 30+ minute timeouts)
docker build -t app-router src/ha-proxy  # 4 seconds (fast)  
docker build -t spa-ui src/spa           # 10-15 minutes (slow due to npm install)
```

### Commands That Do NOT Work
```bash
# These commands FAIL on modern systems:
cd src/api && dotnet restore             # Fails: SDK version mismatch
cd src/api && dotnet build               # Fails: SDK version mismatch
cd src/spa && npm install                # Fails: node-sass compilation errors
```

## Troubleshooting

### Node.js Version Issues
```bash
# If npm install fails, ensure Node.js 12.x:
node --version                           # Should show v12.22.12
nvm use 12.22.12                        # Switch to correct version
```

### .NET Core Build Issues  
```bash
# .NET Core 1.0 SDK not available on modern systems
# Solution: Use Docker or virtual machine with Ubuntu 16.04/CentOS 7
```

### Docker Build Timeouts
```bash
# Increase Docker build timeout to 30+ minutes
docker build --timeout 1800 -t spa-ui src/spa
```

## Development Workflow

1. **Always use Node.js 12.x** for SPA development
2. **Set long timeouts** (30+ minutes) for Docker builds  
3. **Use `--ignore-scripts`** flag with npm install
4. **Test SPA changes** with `npm test` before committing
5. **Validate manually** by running containers and testing user scenarios
6. **Never cancel** long-running builds - they take 10-30 minutes

## Legacy Technology Stack Notice

This repository demonstrates legacy .NET Core 1.0 and React 15.x patterns. Modern development should use:
- .NET 8+ with modern project format
- React 18+ with modern tooling
- Current Node.js LTS versions

For learning purposes, this repository preserves the original technology stack and patterns from 2016-2017 era.