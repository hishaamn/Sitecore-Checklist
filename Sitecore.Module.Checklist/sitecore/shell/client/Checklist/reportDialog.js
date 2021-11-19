(function (Speak) {
    Speak.pageCode(["sitecore", "jquery"],
        function (sitecore, $) {
            return {
                initialized: function () {
                    var app = this;
                    app.on("show", this.loadReport, this);
                },
                loadReport: function () {
                    var reportModel = Sitecore.Speak.app.RecentList.ClickedItem;

                    $.ajax({
                        type: "GET",
                        dataType: "json",
                        data: { reportId: reportModel.ChecklistReportId },
                        url: "/api/checklist/getreportdetail",
                        cache: false,
                        success: function (data) {
                            $(".my-custom-reportpanel").html(data);
                        },
                        error: function () {
                            console.log("There was an error. Try again please!");
                        }
                    });
                }
            };
        },
        "SubAppRendererReportDetail");
})(Sitecore.Speak);