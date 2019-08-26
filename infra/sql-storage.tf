resource "azurerm_sql_server" "chat-app" {
  name                         = "chat-app-db-server"
  resource_group_name          = "${azurerm_resource_group.chat-app.name}"
  location                     = "North Europe"
  version                      = "12.0"
  administrator_login          = "admin4lyfe"
  administrator_login_password = "H,sUZrn[2'&e7e-n"
}

resource "azurerm_sql_database" "chat-app" {
  name                = "chat_app_db"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  location            = "North Europe"
  server_name         = "${azurerm_sql_server.chat-app.name}"
  edition             = "Basic"
}