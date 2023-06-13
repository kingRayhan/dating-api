using System.Net;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace api.Controllers;

public class UsersController : BaseApiController
{
    private DataContext _context;

    private User[] users =
    {
        new User() { Id = 1, UserName = "Rayhan" },
        new User() { Id = 2, UserName = "John" },
        new User() { Id = 3, UserName = "Doe" },
        new User() { Id = 4, UserName = "Jane" },
        new User() { Id = 5, UserName = "Doe" }
    };

    public UsersController(DataContext dataContext)
    {
        this._context = dataContext;
    }

    [HttpGet]
    [DefaultStatusCode(200)]
    public ActionResult<IEnumerable<User>> Getusers()
    {
        // var users = _context.Users.ToList();
        return users;
    }

    [HttpGet("{id:int}")]
    public dynamic GetUser(int id)
    {
        var user = users.FirstOrDefault(user => user.Id == id);

        // check if user is not found then return 404
        return user == null
            ? StatusCode((int)HttpStatusCode.NotFound, new { message = "User not found" })
            : StatusCode((int)HttpStatusCode.Forbidden, user);
    }


    [HttpPost]
    public dynamic CreateUser([FromBody] User user)
    {
        // save user to users
        // return 201
        var newUser = new User()
        {
            Id = users.Length,
            UserName = user.UserName,
        };

        users.Append(newUser);
        return StatusCode((int)HttpStatusCode.Created, new
        {
            message = "New user created",
            data = newUser
        });
    }

    [HttpPatch("{id:int}")]
    public dynamic UpdateUser(int id, [FromBody] JsonPatchDocument patchDocument)
    {
        // Retrieve the existing user from the database based on the provided ID
        // var existingUser = users.FirstOrDefault(user => user.Id == id);
        // if (existingUser == null)
        // {
        //     return NotFound();
        // }
        //
        // // Apply the patch document to the existing user
        // patchDocument.ApplyTo(existingUser, ModelState);
        //
        // // Validate the updated user object
        // TryValidateModel(existingUser);
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        //
        // // Update the user in the database or perform other necessary operations
        //
        return Ok("User updated successfully");
    }
}