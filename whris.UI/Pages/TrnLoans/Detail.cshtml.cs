using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.TrnLoan.Commands;
using whris.Application.CQRS.TrnLoan.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnLoans
{
    [Authorize]
    [Secure("MstEmployeeLoan")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public TrnLoanDetailDto LoanDetail { get; set; } = new TrnLoanDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var loan = new GetTrnLoanById()
            {
                Id = Id
            };

            LoanDetail = await _mediator.Send(loan);
        }

        public async Task OnPostAdd()
        {
            var addLoan = new AddLoan();

            LoanDetail = await _mediator.Send(addLoan);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteLoan = new DeleteLoan()
            {
                Id = id
            };

            await _mediator.Send(deleteLoan);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnLoanDetailDto loan)
        {
            var saveLoan = new SaveLoan()
            {
                Loan = loan
            };

            var resultId = await _mediator.Send(saveLoan);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getLoan = new GetTrnLoanIdByTurnPage()
            {
                Id = id,
                Action = action
            };

            var loanId = await _mediator.Send(getLoan);

            return new JsonResult(new { Id = loanId });
        }

        public async Task<IActionResult> OnPostReadLoanPaymentList([DataSourceRequest] DataSourceRequest request, int employeeLoanId)
        {
            var allLoans = new GetLoanPaymentsView()
            {
                Request = request,
                EmployeeLoanId = employeeLoanId
            };

            return new JsonResult(await _mediator.Send(allLoans));
        }
    }
}
