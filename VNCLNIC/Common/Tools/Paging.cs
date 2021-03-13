using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Common.Tools
{
    public class Paging
    {
        /// <summary>
        /// Khởi tạo
        /// </summary>
        /// <param name="currentPage">Trang hiện tại</param>		
        /// <param name="pageSize">Số dòng muốn hiển thị trên lưới</param>
        /// <param name="datasetSize">Số dòng thực tế của dataset</param>
        /// <param name="basedURL">Đường dẫn cơ sở</param>
        public Paging(int currentPage, int pageSize, int datasetSize, string basedURL)
        {
            //Tính tổng số dòng trên lưới hiện tại (số dòng trên trang* số của trang hiện tại-> số max, số min=số dòng trên trang*(max của trang trước))
            int upperRange = 0;
            int lowerRange = 0;
            lowerRange = pageSize * (currentPage - 1) + 1;
            upperRange = pageSize * currentPage;
            if (lowerRange > datasetSize) lowerRange = datasetSize;
            if (upperRange > datasetSize) upperRange = datasetSize;
            string label = "";

            if (lowerRange < upperRange)
            {
                label = "Hiển thị từ mục " + lowerRange.ToString() + " đến " + upperRange.ToString() + " trong " + datasetSize.ToString() + " mục tìm thấy.";
            }
            else if (lowerRange == upperRange)
            {
                label = "Hiển thị mục thứ " + lowerRange.ToString() + " trong " + datasetSize.ToString() + " mục tìm thấy.";
            }
            //tổng số trang có thể
            int totalPage = (int)(datasetSize / pageSize) + 1;
            // Nếu trang yêu cầu vượt qua tổng số trang thì thiết lập là tổng số trang
            if (currentPage > totalPage) currentPage = totalPage;
            //Nút đầu
            string firstButton = "";
            if (currentPage > 1)
            {
                firstButton = "<li class='paginate_button page-item previous'>" +
                "<a href='?page=1&size=" + pageSize.ToString() + "&" + basedURL + "' data-dt-idx='0' tabindex='0' class='page-link'>Trang đầu</a></li>";
            }
            else
                firstButton = "<li class='paginate_button page-item previous disabled'>" +
                    "<a href='#' data-dt-idx='0' tabindex='0' class='page-link'>Trang đầu</a></li>";
            //nút cuối
            string lastButton = "";
            if (currentPage < totalPage)
            {
                lastButton = "<li class='paginate_button page-item next'>" +
                "<a href='?page=" + totalPage.ToString() + "&size=" + pageSize.ToString() + "&" + basedURL + "' data-dt-idx='0' tabindex='0' class='page-link'>Trang cuối</a></li>";
            }
            else
                lastButton = "<li class='paginate_button page-item next disabled'>" +
                    "<a href='#' data-dt-idx='0' tabindex='0' class='page-link'>Cuối</a></li>";

            //các nút số
            string numButtons = "";
            bool firstDot = false;
            bool lastDot = false;
            for (int i = 2; i < totalPage; i++)
            {
                /*if (currentPage != i)
                {
                    numButtons += "<li class='paginate_button page-item'>" +
                    "<a href='?page=" + i.ToString() + "&size=" + pageSize.ToString() + "&" + basedURL + "' data-dt-idx='0' tabindex='0' class='page-link'>" + i.ToString() + "</a></li>";
                }
                else
                    numButtons += "<li class='paginate_button page-item active'>" +
                        "<a href='#' data-dt-idx='0' tabindex='0' class='page-link'>" + i.ToString() + "</a></li>";*/
                if (currentPage != i)
                {
                    if (i > currentPage - 5 && i < currentPage + 5)
                    {
                        numButtons += "<li class='paginate_button page-item'>" +
                                       "<a href='?page=" + i.ToString() + "&size=" + pageSize.ToString() + "&" + basedURL + "' data-dt-idx='0' tabindex='0' class='page-link'>" + i.ToString() + "</a></li>";
                    }
                    else
                    {
                        if (i > currentPage - 5 && firstDot == false)
                        {
                            numButtons += "<li class='paginate_button page-item'><a class='page-link' href='#'>...</a></li>";
                            firstDot = true;
                        }
                        else if (i < currentPage + 5 && lastDot == false)
                        {
                            numButtons += "<li class='paginate_button page-item'><a class='page-link' href='#'>...</a></li>";
                            lastDot = true;
                        }
                    }
                }
                else
                {
                    numButtons += "<li class='paginate_button page-item active'>" +
                        "<a href='#' data-dt-idx='0' tabindex='0' class='page-link'>" + i.ToString() + "</a></li>";
                }
            }
            //Nhãn góc bên trái

            label = "<div class='col-sm-12 col-md-5'><div class='dataTables_info' id='example1_info' role='status' aria-live='polite'>" + label + "</div></div>";
            //Code html cuối cùng
            string html = "<div class='row'>" + label +
                "<div class='col-sm-12 col-md-7'><div class='dataTables_paginate paging_simple_numbers' id='example1_paginate'><ul class='pagination'>" +
                firstButton + numButtons + lastButton + "</ul></div></div></div>";

            PageModel = new PagingModel() { HTML = html, StartItem = lowerRange, StopItem = upperRange };
        }

        /// <summary>
        /// Khởi tạo Paging cho Ajax
        /// </summary>
        /// <param name="currentPage">Trang hiện tại</param>		
        /// <param name="pageSize">Số dòng muốn hiển thị trên lưới</param>
        /// <param name="datasetSize">Số dòng thực tế của dataset</param>
        /// <param name="jsAction">Action để gọi trang kế</param>
        public Paging(int currentPage, int pageSize, int datasetSize, string jsAction, string additional)
        {
            //Tính tổng số dòng trên lưới hiện tại (số dòng trên trang* số của trang hiện tại-> số max, số min=số dòng trên trang*(max của trang trước))
            int upperRange = 0;
            int lowerRange = 0;
            lowerRange = pageSize * (currentPage - 1) + 1;
            upperRange = pageSize * currentPage;
            if (lowerRange > datasetSize) lowerRange = datasetSize;
            if (upperRange > datasetSize) upperRange = datasetSize;
            string label = "";

            if (lowerRange < upperRange)
            {
                label = "Hiển thị " + lowerRange.ToString() + " - " + upperRange.ToString() + " trong " + datasetSize.ToString() + " mục.";
            }
            else if (lowerRange == upperRange)
            {
                label = "Hiển thị " + lowerRange.ToString() + " trong " + datasetSize.ToString() + " mục.";
            }
            //tổng số trang có thể
            int totalPage = (int)(datasetSize / pageSize) + 1;
            // Nếu trang yêu cầu vượt qua tổng số trang thì thiết lập là tổng số trang
            if (currentPage > totalPage) currentPage = totalPage;

            //các nút số
            string numButtons = "";
            bool firstDot = false;
            bool lastDot = false;
            for (int i = 1; i < totalPage + 1; i++)
            {
                if (currentPage != i)
                {
                    //numButtons += "<li class='page-item'><a class='page-link' href='#' onclick='" + jsAction + "(" + i.ToString ( ) + ");'>" + i.ToString ( ) + "</a></li>";
                    if (i == 1 || i == totalPage)
                    {
                        numButtons += "<li class='page-item'><a class='page-link' href='#' onclick='" + jsAction + "(" + i.ToString() + ");'>" + i.ToString() + "</a></li>";
                    }
                    else if (i > currentPage - 5 && i < currentPage + 5)
                    {
                        numButtons += "<li class='page-item'><a class='page-link' href='#' onclick='" + jsAction + "(" + i.ToString() + ");'>" + i.ToString() + "</a></li>";
                    }
                    else
                    {
                        if (i > currentPage - 5 && firstDot == false)
                        {
                            numButtons += "<li class='page-item'><a class='page-link' href='#'>...</a></li>";
                            firstDot = true;
                        }
                        else if (i < currentPage + 5 && lastDot == false)
                        {
                            numButtons += "<li class='page-item'><a class='page-link' href='#'>...</a></li>";
                            lastDot = true;
                        }
                    }
                }
                else
                {
                    numButtons += "<li class='page-item active'><a class='page-link' href='#'>" + i.ToString() + "</a></li>";
                }
            }

            PageModel = new PagingModel() { HTML = numButtons, StartItem = lowerRange, StopItem = upperRange, Label = label };
        }

        /// <summary>
        /// lấy mã HTML phân trang, chỉ mục...
        /// </summary>
        public PagingModel PageModel
        {
            get; set;
        }


    }
    public class PagingModel
    {
        public int StartItem
        {
            get; set;
        }
        public int StopItem
        {
            get; set;
        }
        public string HTML
        {
            get; set;
        }
        public string Label
        {
            get; set;
        }
    }
    public class GridColumn
    {
        /// <summary>
        /// Tên cột (Biến)
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// Văn bản hiển thị cho cột
        /// </summary>
        public string Text
        {
            get; set;
        }
        /// <summary>
        /// Thứ tự cột trong lưới
        /// </summary>
        public int Order
        {
            get; set;
        }
        /// <summary>
        /// Cho phép sắp xếp hay không
        /// </summary>
        public bool AllowSorting
        {
            get; set;
        }
        /// <summary>
        /// Cho phép hiển thị hay không
        /// </summary>
        public bool AllowDisplay
        {
            get; set;
        }
    }
}