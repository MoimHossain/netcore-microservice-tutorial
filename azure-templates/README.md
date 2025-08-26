# Azure Resource Manager Templates

This directory contains Azure Resource Manager (ARM) templates for deploying the infrastructure required for the netcore-microservice-tutorial project.

## Templates

### Azure Container Registry (ACR)
- `acr-template.json` - Main ARM template for Azure Container Registry
- `acr-parameters.json` - Parameters file for ACR template customization

### Complete Infrastructure
- `main-template.json` - Complete infrastructure template including ACR and supporting resources
- `main-parameters.json` - Parameters file for complete infrastructure

## Deployment

### Prerequisites
- Azure CLI installed and logged in
- Resource Group created

### Deploy Azure Container Registry Only
```bash
az deployment group create \
  --resource-group <your-resource-group> \
  --template-file acr-template.json \
  --parameters @acr-parameters.json
```

### Deploy Complete Infrastructure
```bash
az deployment group create \
  --resource-group <your-resource-group> \
  --template-file main-template.json \
  --parameters @main-parameters.json
```

## Parameters

### ACR Template Parameters
- `registryName` - Name of the Azure Container Registry (must be globally unique)
- `registryLocation` - Azure region for the registry
- `registrySku` - SKU tier (Basic, Standard, Premium)
- `adminUserEnabled` - Enable admin user for the registry

### Main Template Parameters
Includes all ACR parameters plus additional infrastructure configuration options.