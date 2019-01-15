/**
 * 导出模块
 * ES6 模块不是对象，而是通过export命令显式指定输出的代码，再通过import命令输入
 * 总结来说：模块化的好处和问题
 * 1. 可维护性
 * 2. 灵活架构，焦点分离
 * 3. 方便模块间组合、分解
 * 4. 方便单个模块功能调试、升级
 * 5. 多人协作互不干扰
 * 6. 可测试性，可分单元测试；
 * 7. 性能损耗
 * 8. 系统分层，调用链会很长
 * 9. 模块间通信,模块间发送消息会很耗性能
 */
export class Student {
    constructor(homework=[]){
        this.homework = homework;
    }
    study(){
        console.log(this.homework);
    }
}