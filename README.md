# PFM - Personal Finance Management

## Features

-  Import Transactions from CSV file
-  List view of transactions
-  Categorize transaction
-  Split transaction
-  Import categories from CSV file
-  List view of categories
-  Analytics view of transactions
-  Auto categorize transactions

## Importing data from CSV files

Example endpoint :

    [HttpPost("Import")]
    public async Task<IActionResult> ImportTransactions(IFormFile file)
    {
        var res = await _transactionService.ImportFromCSVAsync(file);
        return res.ToOk();
    }

For this feature we are expecting a CSV file from the request body. The file needs to be a valid CSV file and must contain all the required csv headers.
Example headers: 'id, beneficiary-name, date, direction, amount, description, currency, mcc, kind' .
Inside our service class we need to inject the ICSVParser interface from the PFM.Helpers class library which contains one method :

    CSVReponse<TEntity> ParseCSV<TEntity, TMap>(IFormFile file) where TMap : CustomClassMap<TEntity>;

It expects entity type (TEntity) which we are returning example : Transaction , and a CSV mapping profile(TMap).
The CSV mapping profile needs to extend CustomClassMap which extends the ClassMap from CSV Helper nu-get package.

    public class CustomClassMap<T> : ClassMap<T>
    {
        public List<string> Errors { get; set; }
        public CustomClassMap()
        {
            Errors = new List<string>();
        }
    }

The custom class map class , contains one additional field `List<string> Errors` which we are using for row validation errors.
Code example for CSV mapping profile :

    public class TransactionCSVMap : CustomClassMap<Transaction>
    {
        public TransactionCSVMap() : base()
        {
            Map(x => x.Date).Name("date").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Date is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }
                else if (!DateTime.TryParse(field.Field, out DateTime res))
                {
                    Errors.Add($"'{field.Field}' is not a valid date. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }
                return true;
            }).Default(new DateTime(), useOnConversionFailure: true);
        }
    }

We need to specify header name in the `Name()` method , lambda expression for validation , and default value to bypass the default behavior of CSV Helper.

_Note : CSV helper automatically throws an exception for parsing errors , and to bypass this behavior we are setting a default value and the `useOnConversionFailure` needs to be set to true in order to work._

The CSVResponse class returns List of errors , and List of entities :

     public class CSVReponse<T>
    {
        public List<T> Items { get; set; }
        public List<CSVRowError> Errors { get; set; }
    }

Use:

    var csvResponse = _csvParser.ParseCSV<Transaction, TransactionCSVMap>(file);

Here we can check wether the csvResponse is null or contains errors . If csvResponse is null that means the file was invalid or did not contain all the required headers.

Example use for invalid file :

    if (csvResponse is null)
    {
        var error = new CustomFileLoadException($"Error occured while reading file: '{file.FileName}', make sure the file you uploaded is a valid CSV file and you have all the required headers.")
        {
            CsvHeaders = "id, beneficiary-name, date, direction, amount, description, currency, mcc, kind"
        };
        return new Result<ResponseModel>(error);
     }

Example response for invalid file or invalid csv headers :

     {
        "csvHeaders":  "id, beneficiary-name, date, direction, amount, description, currency, mcc, kind",
        "description":  "Error occured while processing your request",
        "message":  "Error occured while reading file: 'README.md', make sure the file you uploaded is a valid CSV file and you have all the required headers.",
        "statusCode":  400
    }

Example use for validation errors :

    if (csvResponse.Errors.Any())
    {
        var exception = new CustomException("One or more validation errors occured while reading file")
        {
            Description = "Problem occured while parsing CSV values , see errors below.",
            StatusCode = HttpStatusCode.BadRequest,
            Errors = csvResponse.Errors
        };
        return new Result<ResponseModel>(exception);
    }

Example response for validation errors :

    {
        "errors":  [
    	    {
    		    "row":  2,
    		    "errors":  [
    				"'daaa' is not a valid transaction direction. Problem occured at Row : 2 , Column : 'direction'",
    				"Description is requiered. Problem occured at Row : 2 , Column : 'description'",
    				"'USD123' is not a valid ISO 4217 currency code. Problem occured at Row : 2 , Column : 'currency'",
    			    "Transaction kind is requiered. Problem occured at Row : 2 , Column : 'kind'"
    			]
    	    }
        ],
        "description":  "Problem occured while parsing CSV values , see errors below.",
        "message":  "One or more validation errors occured while reading file",
        "statusCode":  400

    }

_We are specifying which validation error occurred and which row and column , we can configure how many row errors we are returning in appsettings.json in the `"CSVErrorRows"` environment variable , if none is set default is set to 15_

Finally if the file was valid and had no validation errors, the data would be stored in the database, and we would receive the following example response :

    {
        "message":  "Transactions imported successfully."
    }

Importing data in the database utilizes the Entity framework extensions BulkMerge method , to improve optimization and performance.

    public async Task ImportTransactions(List<Transaction> entities, int batchSize)
        {
            await _dbContext.BulkMergeAsync(entities, options =>
            {
                options.BatchSize = batchSize;
                options.ColumnPrimaryKeyExpression = t => t.Id;
                options.OnMergeUpdateInputExpression = t => new
                {
                    t.BeneficiaryName,
                    t.Description
                };
            });
        }

BulkMerge either inserts or updates , wether an object with that primary key exists in the database.
The OnMergeUpdateInputExpression is used to configure which columns are to be updated.

## List view transactions

This action expects several query parameters :

    public class PagerSorter
    {
        public TransactionKind? TransactionKind { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "date";
        public SortDirection SortOrder { get; set; } = SortDirection.Asc;
    }

_if page is less than 1 default is set to 1 , the max page size can be configured in appsettings.json in the `"MaxItemsForPagination"` variable , if this is not set default page size is set to 10 , if page size from the request exceeds this number page size is set to the value of `"MaxItemsForPagination"` or 10_

Transaction kind needs to be one of the options of the Transactions kind enum :

    public enum TransactionKind
    {
        dep,
        wdw,
        pmt,
        fee,
        inc,
        rev,
        adj,
        lnd,
        lnr,
        fcx,
        aop,
        acl,
        spl,
        sal
    }

Sort Order need to be one of the options of the SortDirection enum :

    public enum SortDirection
    {
        Asc,
        Desc
    }

Example response with query parameters validation errors :

    {
        "message":  "Error occured while processing your request",
        "details":  "The information you provided is not valid. See errors below.",
        "errors":  {
    	    "EndDate":  [
    		    "The value 'test' is not valid for EndDate."
    	    ],
        "TransactionKind":  [
    	    "The value 'pmt123' is not valid for TransactionKind."
    	    ]
        },
        "title":  "One or more validation errors occurred.",
        "status":  400
    }

If no validation errors are present we are returning an object of the following class :

    public class PagedSortedList<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortOrder { get; set; }
        public List<T> Items { get; set; }
    }

## Categorize transaction

     [HttpPost("{transactionId}/Categorize")]
     public async Task<IActionResult> CategorizeTransaction([FromRoute] string transactionId, [FromBody] TransactionCategorizeDto model)
     {
         var res = await _transactionService.CategorizeTransaction(transactionId, model);
         return res.ToOk();
     }

    public class TransactionCategorizeDto
    {
        public string CatCode { get; set; }
    }

This action expect TransactionId as a route parameter , and a category code from the request body.
If transaction with primary key : transactionId or Category with primary key : CatCode do not exist , we are returning a 404 response.

## Split Transaction

     [HttpPost("{transactionId}/Split")]
     public async Task<IActionResult> SplitTransaction([FromRoute] string transactionId, [FromBody] TransactionSplitDto model)
     {
         var res = await _transactionService.SplitTransactionAsync(transactionId, model);
         return res.ToOk();
     }

    public class TransactionSplitDto
    {
        [Required]
        public List<SplitDto> Splits { get; set; }
    }

    public class SplitDto
    {
        [Required]
        public string CatCode { get; set; }
        [Required]
        public double Ammount { get; set; }
    }

This action expect TransactionId as a route parameter , and a Transaction split object which contains a list of Transaction splits from the request body.

If transaction with primary key : transactionId or any category with primary key : CatCode from SplitDto do not exist , we are returning a 404 response.

If there is only one SplitDto , we are respoding with 400 bad request , because we cannot split a transaction on one part , two or more are required.

if the sum of ammounts from the SplitDto's do not match the total ammount from the transaction we are responding with 400 bad request , the sum of split ammounts must match the ammount of the transaction.

## List view categories

     [HttpGet]
     public async Task<IActionResult> GetCategories([FromQuery] string? parentCode = null)
     {
         var res = await _categoryService.GetCategories(parentCode);
         return Ok(res);
     }

If we do not provide a parent code then we are returning the top level categories from the hierarchy.
Otherwise , for example parent code is "B" , we are returning all the sub-categories which have parent code "B"'.

## Analytics view of transactions

    [HttpGet("spending-analytics")]
     public async Task<IActionResult> AnalyticsView([FromQuery] AnalyticsQuery analyticsQuery)
     {
         var res = await _analyticsService.GetTransactionAnalyticsAsync(analyticsQuery);
         return Ok(res);
     }

    public class AnalyticsQuery
    {
        public string? CatCode { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public Direction? Direction { get; set; } = null;
    }

If one or more query parameters validation errors ocur , we are getting the same error response as the Transaction list view.

If CatCode is not specified , we would recieve statistics for all transactions grouped by category , otherwise we would get statistics only for the specified category(example : "B"), we can also concatonate this string example ("A,B,C") , to recieve statistics for the selected categories.

example response :

    public class AnalyticsListDto
    {
        public List<AnalyticsDto> Groups { get; set; }

    }

    public class AnalyticsDto
    {
        public string CatCode { get; set; }
        public double Ammount { get; set; }
        public int Count { get; set; }
    }

    {
        "groups":  [
    	    {
    		    "catCode":  "58",
    		    "ammount":  600.2,
    		    "count":  2
    	    },
    	    {
    		    "catCode":  "5",
    		    "ammount":  5.8,
    		    "count":  3
    	    },
    	    {
    		    "catCode":  "85",
    		    "ammount":  21,
    		    "count":  2
    	    }
        ]
    }

## Auto categorize transactions

We are defining the rules in the `rules.json` file.
Example rule :

      {
        "catCode": "2",
        "parentCode": "B",
        "name": "Auto Insurance",
        "keywords": ["kasko","kasko insurance"],
        "mcc": [6381]
      }

The rule properties are keywords and mcc , the rest is for category data only . If a transaction already has a category then it is skipped , otherwise the rule check comes in play , transaction mcc code has priority if present , the keywords are matched by beneficiary name or description of the transaction.

## Error handling

For error handling we are using the Language common extension nu get package , this allows us to return either the result or the exception that occurred trough the Result struct from said nu get package.
Example:

    public async Task<Result<ResponseModel>> ImportFromCSVAsync(IFormFile file)

When returning values , if no error occurred during execution we return as we normally would , but if we want to return an error we need to do the following :

    var exception = new CustomException("One or more validation errors occured while reading file")
    {
    	Description = "Problem occured while parsing CSV values , see errors below.",
    	StatusCode = HttpStatusCode.BadRequest,
    	Errors = csvResponse.Errors
    };
    return new Result<ResponseModel>(exception);

We are creating an instance of the exception we are not throwing it , then we return an instance of the Result struct from Language common extension and we are passing the exception we just created.

Sending response :

In the code below we have a controller extension method , which we can extend , and reuse trough out our application. For successful requests we can add additional logic , but in this case we are always sending 200 if nothing wrong happened.

    public static class ControllerExtension
    {
        public static IActionResult ToOk<TResult>(this Result<TResult> result)
        {
            return result.Match<IActionResult>(success =>
            {
                return new OkObjectResult(success);
            }, error =>
            {
                if (error is KeyNotFoundException kex)
                {
                    var errorResponse = new ErrorResponse()
                    {
                        StatusCode = 404,
                        Message = kex.Message
                    };
                    return new NotFoundObjectResult(errorResponse);
                }
                return new StatusCodeResult(500);
            });
        }
    }

Example use :

    [HttpPost("Import")]
    public async Task<IActionResult> ImportTransactions(IFormFile file)
    {
        var res = await _transactionService.ImportFromCSVAsync(file);
        return res.ToOk();
    }

Error response models :

We have a base ErrorResponse model and every other error response model extends the base one .

Base Error response model:

    public class ErrorResponse
    {
        public string Description { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ErrorResponse()
        {
            Description = "Error occured while processing your request";
        }
    }

Error response for invalid file :

    public class FileErrorResponse : ErrorResponse
    {
        public string CsvHeaders { get; set; }

        public FileErrorResponse() : base()
        {
            Description = "Error occured while processing your request";
        }
    }

Error response for csv validation errors :

    public class CSVErrorListResponse : ErrorResponse
    {
        public List<CSVRowError> Errors { get; set; }

        public CSVErrorListResponse() : base()
        {

        }
    }

## Testing

We can test all endpoints trough postman , or we can use newman to get an overview of all endpoints trough the newman CLI . We can get a report directly into the terminal or we can get newman to generate a HTML report.

Installing newman

    npm install -g newman

Installing newman HTML reporter

    npm install -g newman-reporter-html

Running newman tests :

First we need to go into the directory where we have exported our postman collection and execute the following command :

    newman run {COLLECTION_NAME}.json

If we are testing on a localhost server , first we need to build the application into release mode, and because newman has problems with localhost servers and the development SSL certificates we need to execute the following command :

    newman run {COLLECTION_NAME}.json --insecure

This will give us an overview in the terminal.

If we want to generate an HTML report we use the following command :

    newman run {COLLECTION_NAME}.json -r html

Same as before if we are on a localhost server we execute the following command :

    newman run {COLLECTION_NAME}.json -r html --insecure

If we want to run multiple itterations we use the following command :

    newman run {COLLECTION_NAME}.json --iteration-count {ITERATION_COUNT} -r html
