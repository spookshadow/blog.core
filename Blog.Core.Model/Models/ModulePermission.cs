﻿using System;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 菜单与按钮关系表
    /// </summary>
    public class ModulePermission
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int ModuleId { get; set; }
        /// <summary>
        /// 按钮ID
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// 创建ID
        /// </summary>
        public int? CreateId { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 修改ID
        /// </summary>
        public int? ModifyId { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        public string ModifyBy { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        ///模块
        /// </summary>
        public virtual Module Module { get; set; }
        /// <summary>
        ///权限
        /// </summary>
        public virtual Permission Permission { get; set; }
    }
}