<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        html, body {
            height: 100%;
            width: 100%;
            margin: 0;
            padding: 0;
        }

        body {
            line-height: 1.5;
            font-family: 微软雅黑;
            background: -webkit-gradient(linear, left top, left bottom, from(#eee), to(#fff));
        }

        .nav {
            border: solid 1px #B8D4B8;
            margin-bottom: 20px;
            border-radius: 5px;
            background-color: #fff;
        }

            .nav:hover {
                box-shadow: 0 5px 30px #999;
            }

            .nav h3 {
                margin: 0;
                background-color: #836FFF;
                padding: 5px 10px;
                font-weight: normal;
                font-size: 16px;
                color: white;
            }

                .nav h3 i {
                    font-style: normal;
                    padding-left: 10px;
                    color: white;
                }

        .clear {
            clear: both;
            height: 15px;
        }

        .nav a {
            float: left;
            width: 150px;
            margin-top: 15px;
            margin-left: 10px;
            text-align: center;
            text-decoration: none;
        }

            .nav a:hover {
                text-decoration: none;
                color: red;
            }

            .nav a i {
                font-style: normal;
                color: #999;
                font-size: 9px;
                position: relative;
                top: -10px;
                left: 0px;
                padding: 1px 3px;
            }
    </style>
    <script type="text/javascript" src="jquery-1.7.1.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.nav a').each(function () {
                var href = $(this).attr('href');
                var count = window.localStorage.getItem(href);
                count = count || 0;
                $(this).append('<i>' + count + '</i>');
                $(this).attr('target', '_blank');
                $(this).click(function () {
                    var count = parseInt($(this).find('i').html()) + 1;
                    $(this).find('i').html(count);
                    window.localStorage.setItem(href, count);
                    console.log(count);
                });
                $(this).hover(function () {
                    $(this).parents('.nav').find('h3 i').html($(this).attr('href'));
                }, function () {
                    $(this).parents('.nav').find('h3 i').html('');
                });
            });
        });
    </script>
</head>
<body>

            <div style="max-width: 1000px; margin: 10px auto;">

                <div class="nav">
                    <h3>数据查询 <i></i></h3>
                    <a href="ruyuanchuyuan.html">入院出院</a>
                    <a href="menzhenliang.html">门诊量</a>

                    <div class="clear"></div>
                </div>

            </div>

</body>
</html>
