# Lesson 11 – ASP.NET Core MVC Helper Methods

This project contains exercises focused on working with **ASP.NET Core MVC**, with particular focus on HTML Helper methods, forms, HTTP GET/POST requests, strongly typed models, and session state.

The exercises are part of my Datamatiker coursework and are used to explore how data moves between the **View** and **Controller** in an MVC application.

---

## Exercise 1 – Country Selector

The first exercise implements a country selector where the user can:

- Select a country from a dropdown list
- Display the corresponding country code
- Add new countries
- Preserve newly added countries between requests
- Prevent duplicate country codes

I expanded the original exercise with some additional functionality and styling while keeping the required MVC concepts intact.

---

### Country Model

Instead of only storing `SelectListItem` objects, the application uses a `CountryItem` model to represent the individual countries.

The country information can then be converted into `SelectListItem` objects when needed by the View.

This keeps the actual country data separate from the way the dropdown is rendered.

---

## HTML Helpers

One of the main subjects of the exercise is using **HTML Helper methods** to generate HTML from Razor.

The country dropdown is generated using `Html.DropDownList`:

```csharp
@Html.DropDownList(
    "SelectedCountry",
    Model.OrderBy(c => c.Name)
        .Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Code
        }),
    "-- Select a country --",
    new { @class = "form-select", id = "countryDropdown" }
)
```

Each `CountryItem` is converted into a `SelectListItem`.

The:

```csharp
Text = c.Name
```

represents what the user sees in the dropdown, while:

```csharp
Value = c.Code
```

represents the value that is submitted to the server.

For example:

```text
Denmark -> DK
Germany -> DE
Iceland -> IS
```

---

## GET Requests

Selecting a country only requests information and does not modify the state of the application.

The form therefore uses an HTTP **GET** request:

```csharp
@using (Html.BeginForm("Index", "Exercise1", FormMethod.Get))
{
    ...
}
```

When the form is submitted, the selected value becomes part of the URL.

For example:

```text
?SelectedCountry=DK
```

The controller can receive this value through ASP.NET Core's model binding:

```csharp
[HttpGet]
public IActionResult Index(string? selectedCountry)
{
    ViewBag.CountryCode = selectedCountry;

    return View(_countries);
}
```

The form field:

```text
SelectedCountry
```

matches the controller parameter:

```csharp
selectedCountry
```

ASP.NET Core can therefore automatically bind the submitted value to the parameter.

The parameter is declared as:

```csharp
string?
```

because no country has necessarily been selected when the page is initially opened.

---

## ViewBag

The selected country code is passed from the Controller to the View using `ViewBag`:

```csharp
ViewBag.CountryCode = selectedCountry;
```

The View can then display the value:

```csharp
<input type="text"
       id="countryCode"
       value="@ViewBag.CountryCode"
       readonly />
```

This provided a simple example of passing additional information from a Controller to a View without adding it directly to the model.

---

## POST Requests

The second form allows the user to add a new country.

Unlike selecting a country, adding a country **changes the state of the application**.

The form therefore uses HTTP **POST**.

The POST request is handled by another `Index` action:

```csharp
[HttpPost]
public IActionResult Index(IFormCollection formData)
{
    ...
}
```

Even though both controller methods are named `Index`, ASP.NET Core can distinguish them using:

```csharp
[HttpGet]
```

and:

```csharp
[HttpPost]
```

The submitted form values can be retrieved through `IFormCollection`.

For example:

```csharp
string countryName = formData["CountryName"].ToString();

string countryCode = formData["CountryCode"].ToString();
```

---

## Input Normalization

Before storing a new country, the submitted values can be normalized.

For example:

```csharp
string countryName = formData["CountryName"]
    .ToString()
    .Trim();

string countryCode = formData["CountryCode"]
    .ToString()
    .Trim()
    .ToUpperInvariant();
```

`Trim()` removes unnecessary whitespace from the beginning and end of the input.

For the country code, `ToUpperInvariant()` ensures that:

```text
dk
DK
dK
Dk
```

are all stored consistently as:

```text
DK
```

This also makes checking for duplicate country codes easier.

---

## Preventing Duplicate Countries

Before adding a country, LINQ's `Any()` method can be used to determine whether the country code already exists:

```csharp
if (!_countries.Any(c => c.Code == countryCode))
{
    CountryItem newCountry = new CountryItem
    {
        Name = countryName,
        Code = countryCode
    };

    _countries.Add(newCountry);
}
```

The expression:

```csharp
c => c.Code == countryCode
```

is evaluated for the countries in the collection.

`Any()` returns `true` as soon as a matching country is found.

This prevents multiple countries with the same country code from being added.

---

# HTTP Is Stateless

One important problem became apparent when implementing the POST functionality.

Initially, adding a country worked inside the POST request:

```csharp
_countries.Add(newCountry);
```

However, after redirecting back to the page, the newly added country disappeared.

This happens because HTTP is **stateless**.

A request does not automatically remember modifications made during a previous request.

For example:

```text
POST Request
    ↓
Country added to _countries
    ↓
Redirect
    ↓
New GET Request
    ↓
New request context
    ↓
Original country data is loaded
```

Some form of persistent state is therefore required if the newly added countries should survive between requests.

For this exercise, that state is provided using **Session**.

---

# Session State

ASP.NET Core Session allows information to be associated with a user's session across multiple HTTP requests.

Session support is registered in `Program.cs`:

```csharp
builder.Services.AddSession();
```

The Session middleware must also be added to the HTTP request pipeline:

```csharp
app.UseRouting();

app.UseSession();

app.UseAuthorization();
```

These two lines serve different purposes.

```csharp
builder.Services.AddSession();
```

registers the services required for Session.

Meanwhile:

```csharp
app.UseSession();
```

adds Session middleware to the application's request pipeline.

---

## Storing the Country List

ASP.NET Core Session does not directly store arbitrary objects such as:

```csharp
List<CountryItem>
```

The list is therefore serialized into JSON before being stored.

```csharp
string countriesJson =
    JsonSerializer.Serialize(_countries);

HttpContext.Session.SetString(
    "Countries",
    countriesJson
);
```

Conceptually:

```text
List<CountryItem>
        ↓
JsonSerializer.Serialize()
        ↓
JSON string
        ↓
Session
```

---

## Retrieving the Country List

During a later request, the JSON string can be retrieved again:

```csharp
string? countriesJson =
    HttpContext.Session.GetString("Countries");
```

The value is nullable because `"Countries"` might not yet exist in the Session.

If the value exists, it can be deserialized back into a country list:

```csharp
if (countriesJson != null)
{
    _countries =
        JsonSerializer.Deserialize<List<CountryItem>>(countriesJson)!;
}
```

The complete process therefore becomes:

```text
List<CountryItem>
       ↓
     JSON
       ↓
    Session
       ↓
     JSON
       ↓
List<CountryItem>
```

This allows newly added countries to remain available during later requests in the same session.

---

# POST / Redirect / GET

After processing the POST request, the application redirects back to the GET action:

```csharp
return RedirectToAction(
    "Index",
    new { selectedCountry = countryCode }
);
```

This results in the following request flow:

```text
User submits Add Country form
          ↓
       HTTP POST
          ↓
Controller processes the data
          ↓
Country stored in Session
          ↓
   RedirectToAction()
          ↓
        HTTP GET
          ↓
Country list retrieved from Session
          ↓
       View rendered
```

This is known as the **Post/Redirect/Get (PRG)** pattern.

One advantage is that refreshing the resulting page performs another GET request rather than resubmitting the previous POST request.

The newly added country code is also supplied to the GET request:

```csharp
new { selectedCountry = countryCode }
```

which produces a URL parameter such as:

```text
?selectedCountry=IS
```

---

# Optional Sorting Helper

The exercise also introduces the idea of extracting functionality into a static utility/helper method.

An `Infrastructure` folder was therefore created containing:

```text
Infrastructure/
└── Utilities.cs
```

The helper demonstrates how the country sorting could be moved away from the View:

```csharp
public static class Utilities
{
    public static List<CountryItem> SortCountryList(
        List<CountryItem> countries)
    {
        return countries
            .OrderBy(c => c.Name)
            .ToList();
    }
}
```

Because `Utilities` is a static class, an instance does not have to be created.

Instead of:

```csharp
Utilities utilities = new Utilities();
```

the method can be accessed directly through the class:

```csharp
_countries = Utilities.SortCountryList(_countries);
```

This is useful for functionality that does not require an individual object's state.

For this implementation, the helper is primarily included to demonstrate the concept.

The active implementation keeps the simple sorting directly when generating the dropdown:

```csharp
Model.OrderBy(c => c.Name)
```

This keeps the current implementation concise while still demonstrating how the functionality could be extracted into a reusable helper.

---

# Additional Functionality

The implementation of Exercise 1 was expanded beyond the original requirements.

Some additional functionality includes:

- A custom `CountryItem` model
- Alphabetical country sorting
- Bootstrap styling
- Dynamic updating of country information
- Country-specific information
- External country information
- An animated greeting/message
- Client-side JavaScript functionality

These additions were made while retaining the server-side MVC functionality required by the exercise.

---

# Exercise 2 – Parking Ticket Machine

_Not implemented yet._

Exercise 2 will focus on **templated HTML Helper methods** and strongly typed models by implementing a simulated parking ticket machine.

Topics will include:

- Strongly typed Views
- `EditorFor`
- `EditorForModel`
- `LabelFor`
- `DisplayFor`
- Model metadata
- Form processing
- Multiple submit buttons

---

# Exercise 3 – Breakfast Order

_Not implemented yet._

Exercise 3 will continue working with form Helper methods by implementing a breakfast ordering form and receipt.

---

# Key Concepts Learned

Exercise 1 introduced and reinforced several ASP.NET Core MVC concepts:

- MVC Controllers and Views
- Razor
- HTML Helper methods
- `Html.BeginForm`
- `Html.DropDownList`
- HTTP GET
- HTTP POST
- Model binding
- `IFormCollection`
- `ViewBag`
- Nullable values
- HTTP statelessness
- Session state
- Middleware
- JSON serialization
- JSON deserialization
- Post/Redirect/Get
- LINQ `Select`
- LINQ `OrderBy`
- LINQ `Any`
- Static classes
- Static helper methods
- Input normalization
- Basic duplicate prevention

---

## Technologies

- C#
- ASP.NET Core MVC
- Razor
- HTML
- CSS
- Bootstrap
- JavaScript
- LINQ
- JSON
- JetBrains Rider

---

## Status

🚧 **Work in progress**

- [x] Exercise 1 – Country Selector
- [x] GET country selection
- [x] POST country creation
- [x] Session persistence
- [x] Duplicate country-code handling
- [x] Optional static sorting helper
- [ ] Exercise 2 – Parking Ticket Machine
- [ ] Exercise 3 – Breakfast Order
