# general info

## tiers
standard
premium ( includes hardware security modules )

## active directory registration
all apps that access keyvault values must be registered with azure active directory. 
When you have code that needs to access or modify resources, you must set up an Azure Active Directory 
(AD) application. You can then assign the required permissions to the AD application. This approach is 
preferable to running the app under your own credentials because you can assign permissions to the app 
identity that are different than your own permissions. Typically, these permissions are restricted to 
exactly what the app needs to do.

## max keyvault name
maximum keyvault name allowed is 24



# powershell

## create a new keyvault through powershell
New-AzureRmKeyVault -VaultName 'ContosoKeyVault' -ResourceGroupName 'ContosoResourceGroup' -Location 'East US'

## create a software protected key
$key = Add-AzureKeyVaultKey -VaultName 'ContosoKeyVault' -Name 'ContosoFirstKey' -Destination 'Software'

## store your own pfx file
$securepfxpwd = ConvertTo-SecureString –String '123' –AsPlainText –Force  // This stores the password 123 in the variable $securepfxpwd
$key = Add-AzureKeyVaultKey -VaultName 'ContosoKeyVault' -Name 'ContosoImportedPFX' -KeyFilePath 'c:\softkey.pfx' -KeyFilePassword $securepfxpwd

## store a sectret phrase
$secretvalue = ConvertTo-SecureString 'Pa$$w0rd' -AsPlainText -Force
$secret = Set-AzureKeyVaultSecret -VaultName 'ContosoKeyVault' -Name 'SQLPassword' -SecretValue $secretvalue

## view secret from vault
(get-azurekeyvaultsecret -vaultName "Contosokeyvault" -name "SQLPassword").SecretValueText

## delete keyvault
Remove-AzureRmKeyVault -VaultName 'ContosoKeyVault'

