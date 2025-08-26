# Deploy complete infrastructure for netcore-microservice-tutorial
# Usage: .\Deploy-Infrastructure.ps1 -ResourceGroupName "my-resource-group" [-TemplateFile "main-template.json"] [-ParametersFile "main-parameters.json"]

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$false)]
    [string]$TemplateFile = "main-template.json",
    
    [Parameter(Mandatory=$false)]
    [string]$ParametersFile = "main-parameters.json"
)

# Check if Azure CLI is installed
try {
    az version | Out-Null
} catch {
    Write-Error "Azure CLI is not installed. Please install it first."
    exit 1
}

# Check if user is logged in
try {
    az account show | Out-Null
} catch {
    Write-Error "Not logged in to Azure. Please run 'az login' first."
    exit 1
}

# Get the directory where the script is located
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Check if template and parameters files exist
$TemplateFilePath = Join-Path $ScriptDir $TemplateFile
$ParametersFilePath = Join-Path $ScriptDir $ParametersFile

if (-not (Test-Path $TemplateFilePath)) {
    Write-Error "Template file '$TemplateFile' not found in $ScriptDir"
    exit 1
}

if (-not (Test-Path $ParametersFilePath)) {
    Write-Error "Parameters file '$ParametersFile' not found in $ScriptDir"
    exit 1
}

Write-Host "Deploying complete infrastructure for netcore-microservice-tutorial..." -ForegroundColor Green
Write-Host "Resource Group: $ResourceGroupName"
Write-Host "Template: $TemplateFile"
Write-Host "Parameters: $ParametersFile"
Write-Host ""

# Check if resource group exists
$rgExists = az group exists --name $ResourceGroupName | ConvertFrom-Json

if (-not $rgExists) {
    Write-Host "Resource group '$ResourceGroupName' does not exist." -ForegroundColor Yellow
    $createRg = Read-Host "Do you want to create it? [y/N]"
    
    if ($createRg -eq "y" -or $createRg -eq "Y") {
        Write-Host "Creating resource group '$ResourceGroupName'..." -ForegroundColor Yellow
        az group create --name $ResourceGroupName --location "East US"
    } else {
        Write-Host "Exiting..." -ForegroundColor Red
        exit 1
    }
}

# Deploy the template
Write-Host "Starting deployment..." -ForegroundColor Green
$DeploymentName = "infrastructure-deployment-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

try {
    az deployment group create `
        --resource-group $ResourceGroupName `
        --name $DeploymentName `
        --template-file $TemplateFilePath `
        --parameters "@$ParametersFilePath" `
        --verbose

    Write-Host ""
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host "Deployment name: $DeploymentName"
    
    # Show deployment outputs
    Write-Host ""
    Write-Host "Deployment outputs:" -ForegroundColor Cyan
    az deployment group show `
        --resource-group $ResourceGroupName `
        --name $DeploymentName `
        --query properties.outputs `
        --output table
        
    Write-Host ""
    Write-Host "🔍 Resources created:" -ForegroundColor Cyan
    az resource list `
        --resource-group $ResourceGroupName `
        --output table
        
} catch {
    Write-Host "❌ Deployment failed!" -ForegroundColor Red
    Write-Error $_.Exception.Message
    exit 1
}