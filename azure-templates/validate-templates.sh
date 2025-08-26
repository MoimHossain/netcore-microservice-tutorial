#!/bin/bash

# Validate ARM templates for netcore-microservice-tutorial
# Usage: ./validate-templates.sh [resource-group-name]

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
RESOURCE_GROUP_NAME=${1:-"validation-rg-temp"}

# Get the directory where the script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Validating ARM templates for netcore-microservice-tutorial..."
echo ""

# Create a temporary resource group for validation if it doesn't exist
if ! az group show --name "$RESOURCE_GROUP_NAME" &> /dev/null; then
    echo "Creating temporary resource group for validation: $RESOURCE_GROUP_NAME"
    az group create --name "$RESOURCE_GROUP_NAME" --location "East US" --tags "purpose=validation" "temporary=true"
    CLEANUP_RG=true
else
    CLEANUP_RG=false
fi

# Validate ACR template
echo "🔍 Validating ACR template..."
az deployment group validate \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --template-file "$SCRIPT_DIR/acr-template.json" \
    --parameters "@$SCRIPT_DIR/acr-parameters.json" \
    --verbose

if [ $? -eq 0 ]; then
    echo "✅ ACR template validation passed!"
else
    echo "❌ ACR template validation failed!"
    exit 1
fi

echo ""

# Validate main infrastructure template
echo "🔍 Validating main infrastructure template..."
az deployment group validate \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --template-file "$SCRIPT_DIR/main-template.json" \
    --parameters "@$SCRIPT_DIR/main-parameters.json" \
    --verbose

if [ $? -eq 0 ]; then
    echo "✅ Main infrastructure template validation passed!"
else
    echo "❌ Main infrastructure template validation failed!"
    exit 1
fi

# Cleanup temporary resource group if we created it
if [ "$CLEANUP_RG" = true ]; then
    echo ""
    echo "🧹 Cleaning up temporary resource group..."
    az group delete --name "$RESOURCE_GROUP_NAME" --yes --no-wait
fi

echo ""
echo "🎉 All templates validated successfully!"
echo ""
echo "Next steps:"
echo "1. Deploy ACR only: ./deploy-acr.sh <resource-group-name>"
echo "2. Deploy complete infrastructure: ./deploy-infrastructure.sh <resource-group-name>"