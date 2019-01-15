(function () {
    'use strict';

    /**
     * 导出模块
     * ES6 模块不是对象，而是通过export命令显式指定输出的代码，再通过import命令输入
     */
    class Student {
        constructor(homework=[]){
            this.homework = homework;
        }
        study(){
            console.log(this.homework);
        }
    }

    /**
     * 入口文件，因为浏览器现在还不能直接运行模块化代码，所以需要打包
     * 
     * 打包流程
     * 1. 安装npm 或者 cnpm 
     * 2. 全局安装 rollup.js
     *    $ cnpm install --global rollup
     * 3. 切换工作目录
     *    cd [当前文件夹]
     * 4. 执行打包
     *    $ rollup main.js --format iife --output dist/bundle.js
     * 在HTML中引用生成的脚本
     *    <script src="bundle.js"></script>
     */

    const st = new Student([
        'blog',
        'api',
        'vue'
    ]);

    st.study();

}());
