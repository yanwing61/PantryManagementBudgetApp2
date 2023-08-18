## Pantry Management and Budgeting App

Welcome to the Pantry Management and Budgeting App! This web application is designed to help you manage your inventory of pantry items, track your expenses, and maintain a budget.

### Custom Entities

- **Period:** Represents a time period within the budget management system. Other entities like balances and cashflows are associated with specific periods.

- **Balance:** Represents the financial balance during a specific period. It's related to a period and provides information about the financial state.

- **Cashflow:** Represents the flow of money in and out of the budget during a specific period.

- **PantryItem:** Represents an item in the pantry that needs to be tracked. This can include attributes like quantity, name, etc.

- **Inventory:** Represents the inventory of pantry items, indicating the available quantity of each item.

- **Tag:** Represents tags that can be associated with pantry items, allowing categorization and filtering.

- **Purchase:** Represents a purchase transaction, including details about the purchased items and costs.


### Additional Features

**Authentication:** Authentication and roles were added to the app to ensure that users have access to different permission levels depending on their assigned roles.
- User - Read and Add only
- Admin - Full CRUD

**Styling:** Custom styling and layout were implemented to the app.
