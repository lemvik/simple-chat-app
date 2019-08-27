resource "azurerm_virtual_network" "chat-app" {
  name                = "chat-app-network"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  location            = "${azurerm_resource_group.chat-app.location}"
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_subnet" "chat-app-subnet" {
  name                 = "chat-app-subnet"
  resource_group_name  = "${azurerm_resource_group.chat-app.name}"
  virtual_network_name = "${azurerm_virtual_network.chat-app.name}"
  address_prefix       = "10.0.0.0/24"
}

resource "azurerm_public_ip" "chat-app" { # public ip and dns allocation
  name                         = "chat-app-public-ip"
  location                     = "${azurerm_resource_group.chat-app.location}"
  resource_group_name          = "${azurerm_resource_group.chat-app.name}"
  domain_name_label            = "${azurerm_resource_group.chat-app.name}"
  public_ip_address_allocation = "dynamic"
}
