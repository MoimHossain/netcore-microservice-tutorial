#!/bin/bash

# Deploy complete infrastructure for netcore-microservice-tutorial
# Usage: ./deploy-infrastructure.sh <resource-group-name> [template-file] [parameters-file]

set -e

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "Error: Azure CLI is not installed. Please install it first."
    exit 1
fi

# Check if user is logged in
if ! az account show &> /dev/null; then
    echo "Error: Not logged in to Azure. Please run 'az login' first."
    exit 1
fi

# Parameters
RESOURCE_GROUP_NAME=${1:-""}
TEMPLATE_FILE=${2:-"main-template.json"}
PARAMETERS_FILE=${3:-"main-parameters.json"}

if [ -z "$RESOURCE_GROUP_NAME" ]; then
    echo "Usage: $0 <resource-group-name> [template-file] [parameters-file]"
    echo "Example: $0 my-resource-group"
    exit 1
fi

# Get the directory where the script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check if template and parameters files exist
if [ ! -f "$SCRIPT_DIR/$TEMPLATE_FILE" ]; then
    echo "Error: Template file '$TEMPLATE_FILE' not found in $SCRIPT_DIR"
    exit 1
fi

if [ ! -f "$SCRIPT_DIR/$PARAMETERS_FILE" ]; then
    echo "Error: Parameters file '$PARAMETERS_FILE' not found in $SCRIPT_DIR"
    exit 1
fi

echo "Deploying complete infrastructure for netcore-microservice-tutorial..."
echo "Resource Group: $RESOURCE_GROUP_NAME"
echo "Template: $TEMPLATE_FILE"
echo "Parameters: $PARAMETERS_FILE"
echo ""

# Check if resource group exists
if ! az group show --name "$RESOURCE_GROUP_NAME" &> /dev/null; then
    echo "Resource group '$RESOURCE_GROUP_NAME' does not exist."
    read -p "Do you want to create it? [y/N]: " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "Creating resource group '$RESOURCE_GROUP_NAME'..."
        az group create --name "$RESOURCE_GROUP_NAME" --location "East US"
    else
        echo "Exiting..."
        exit 1
    fi
fi

# Deploy the template
echo "Starting deployment..."
DEPLOYMENT_NAME="infrastructure-deployment-$(date +%Y%m%d-%H%M%S)"

az deployment group create \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --name "$DEPLOYMENT_NAME" \
    --template-file "$SCRIPT_DIR/$TEMPLATE_FILE" \
    --parameters "@$SCRIPT_DIR/$PARAMETERS_FILE" \
    --verbose

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Deployment completed successfully!"
    echo "Deployment name: $DEPLOYMENT_NAME"
    
    # Show deployment outputs
    echo ""
    echo "Deployment outputs:"
    az deployment group show \
        --resource-group "$RESOURCE_GROUP_NAME" \
        --name "$DEPLOYMENT_NAME" \
        --query properties.outputs \
        --output table
        
    echo ""
    echo "🔍 Resources created:"
    az resource list \
        --resource-group "$RESOURCE_GROUP_NAME" \
        --output table
else
    echo "❌ Deployment failed!"
    exit 1
fi