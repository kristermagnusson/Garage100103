using AutoMapper;
using GarageV3.Core.Models;
using GarageV3.Core.ViewModels;
using GarageV3.Data;
using GarageV3.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GarageV3.Client.Controllers
{
    public class SearchController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;


        private int _garageCapacity;
        private int _ticketBasePrice;
        private string _currency;


        public SearchController(GarageDBContext context, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = new UnitOfWork(context);
            _configuration = configuration;

            _garageCapacity = int.Parse(configuration["GarageCapacity"]);
            _ticketBasePrice = int.Parse(_configuration["TicketBasePrice"]);
            _currency = _configuration["Currency"];
        }

        [HttpGet]
        [ActionName("Load")]
        public async Task<IActionResult> LoadAsync()
        {
            var model = SetLoadOption(new SearchViewModel());

            return await Task.FromResult(View("../Search/SearchMain", model));
        }

        [HttpPost]
        [ActionName("FindVehicle")]
        public async Task<IActionResult> FindVehicleAsync(SearchViewModel model)
        {
            ModelState.Clear();

            var result = new List<VehicleViewModel>();

            if (string.IsNullOrWhiteSpace(model.SearchOption))
            {
                result = await _mapper.ProjectTo<VehicleViewModel>(_unitOfWork.VehicleRepo.GetAll()).ToListAsync();
            }
            else
            {
                Expression<Func<Vehicle, bool>> predicate = q =>
                    q.RegNr.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.Brand.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.Model.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.Color.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.VehicleType.VType.ToLower().Contains(model.SearchOption.ToLower());

                result = await _mapper.ProjectTo<VehicleViewModel>(_unitOfWork.VehicleRepo.Find(predicate)).ToListAsync();
            }


            var userInfo = result.Any() ? "" : "Inga poster funna";

            model.AltSearch = AltSearch.Vehicle;
            var _model = SetLoadOption(model, userInfo);

            _model.Vehicles = result;

            _model.SearchOption = string.Empty;

            return await Task.FromResult(View("../Search/SearchMain", _model));
        }



        [HttpPost]
        [ActionName("FindMemberShip")]
        public async Task<IActionResult> FindMemberShipAsync(SearchViewModel model)
        {
            ModelState.Clear();

            var result = new List<MemberShipsViewModel>();

            if (string.IsNullOrWhiteSpace(model.SearchOption))
            {
                result = await _mapper.ProjectTo<MemberShipsViewModel>(_unitOfWork.MembershipRepo.GetAll()).ToListAsync();
            }
            else
            {
                Expression<Func<Membership, bool>> predicate = q =>
                    q.MembershipCategory.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.Owner.FirstName.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.Owner.LastName.ToLower().Contains(model.SearchOption.ToLower());

                var test = await _unitOfWork.MembershipRepo.Find(predicate).ToListAsync();

                result = await _mapper.ProjectTo<MemberShipsViewModel>(_unitOfWork.MembershipRepo.Find(predicate)).ToListAsync();
            }


            var userInfo = result.Any() ? "" : "Inga poster funna";

            model.AltSearch = AltSearch.MemberShip;
            var _model = SetLoadOption(model, userInfo);

            _model.MemberShips = result;


            _model.SearchOption = string.Empty;

            return await Task.FromResult(View("../Search/SearchMain", _model));
        }


        [HttpPost]
        [ActionName("FindOwner")]
        public async Task<IActionResult> FindOwnerAsync(SearchViewModel model)
        {
            ModelState.Clear();

            var result = new List<OwnerViewModel>();

            if (string.IsNullOrWhiteSpace(model.SearchOption))
            {
                result = await _mapper.ProjectTo<OwnerViewModel>(_unitOfWork.OwnerRepo.GetAll()).ToListAsync();
            }
            else
            {
                Expression<Func<Owner, bool>> predicate = q =>
                    q.FirstName.ToLower().Contains(model.SearchOption.ToLower()) ||
                    q.LastName.ToLower().Contains(model.SearchOption.ToLower());

                var test = await _unitOfWork.OwnerRepo.Find(predicate).ToListAsync();

                result = await _mapper.ProjectTo<OwnerViewModel>(_unitOfWork.OwnerRepo.Find(predicate)).ToListAsync();
            }


            var userInfo = result.Any() ? "" : "Inga poster funna";

            model.AltSearch = AltSearch.Owner;
            var _model = SetLoadOption(model, userInfo);

            _model.Owners = result;

            _model.SearchOption = string.Empty;



            return await Task.FromResult(View("../Search/SearchMain", _model));
        }


        [HttpPost]
        [ActionName("SelectOption")]
        public async Task<IActionResult> SelectOptionAsync(SearchViewModel model)
        {
            var _model = SetLoadOption(model);

            _model.SubTitle = "Utför sökningar baserat på dina kriterier";

            return await Task.FromResult(View("../Search/SearchMain", _model));
        }


        private SearchViewModel SetLoadOption(SearchViewModel _model, string userInfo = "")
        {
            switch (_model.AltSearch)
            {
                case AltSearch.MemberShip:
                    _model.HeadLine = "Sök i medlemsregistret";
                    break;
                case AltSearch.Owner:
                    _model.HeadLine = "Sök bland ägare";
                    break;
                case AltSearch.Vehicle:
                    _model.HeadLine = "Sök i garaget";
                    break;
                default:
                    _model.HeadLine = "Sök i databasen";
                    _model.UserInfo = "Välj ett alternativ från dropdown";
                    break;
            }



            _model.UserInfo = !string.IsNullOrWhiteSpace(_model.UserInfo) ? _model.UserInfo : "";

            return _model;
        }
    }
}
