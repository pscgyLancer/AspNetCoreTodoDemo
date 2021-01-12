using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTodo.Controllers
{
	[Authorize]
	public class TodoController:Controller
	{
		private readonly ITodoItemService _todoItemService;
		private readonly UserManager<ApplicationUser> _userManager;
		//ʹ��ApplicationUser�Զ���ģ��,��Ҫ�޸�startup.cs��_LoginPartial.cshtml��IdentityUser��ΪApplicationUser
		//https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-5.0#customize-the-model

		public TodoController(ITodoItemService todoItemService, UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }


        /// <summary>
        /// ��ҳ��ͼ
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null) return Challenge();

			var items = await _todoItemService
				.GetIncompleteItemsAsync(currentUser);


			var model= new TodoViewModel()
			{
				Items = items
			};
			return View(model);
		}


		/// <summary>
		/// ��Ӵ���
		/// </summary>
		/// <param name="newItem"></param>
		/// <returns></returns>
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddItem(TodoItem newItem)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Index");
			}
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null) return Challenge();

			var successful = await _todoItemService.AddItemAsync(newItem, currentUser);
			if (!successful)
			{
				return BadRequest("Could not add item.");
			}

			return RedirectToAction("Index");
		}
		
		/// <summary>
		/// ��ɴ���
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkDone(Guid id)
		{
			if (id == Guid.Empty)
			{
				return RedirectToAction("Index");
			}

			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null) return Challenge();

			var successful = await _todoItemService.MarkDoneAsync(id, currentUser);
			if (!successful)
			{
				return BadRequest("Could not mark item as done.");
			}

			return RedirectToAction("Index");
		}



	}
}
