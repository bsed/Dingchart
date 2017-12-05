using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Models.Tables;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Business.Cache;
using cn.lds.chatcore.pcw.Business;

namespace cn.lds.chatcore.pcw.DataSqlite {
class MucMembersDao : BaseDao {
    private static MucMembersDao instance = null;
    public static MucMembersDao getInstance() {
        if (instance == null) {
            instance = new MucMembersDao();
        }
        return instance;
    }

    private GroupCacheEntity getCacheEntity(String idOrNo) {
        GroupCacheEntity cgroup = CacheHelper.getInstance().getGroup(idOrNo);
        if (cgroup == null) {
            return null;
        }
        if (cgroup.table == null) {
            return null;
        }
        return cgroup;
    }

    /// <summary>
    /// 更新信息
    /// </summary>
    /// <param Name="table"></param>
    /// <returns></returns>
    public int save(MucMembersTable table) {
        GroupCacheEntity cgroup = this.getCacheEntity(table.mucno);

        int count = -1;
        try {
            Dictionary<string, object> entity = new Dictionary<string, object>();
            entity.Add("mucId", table.mucId);
            entity.Add("mucno", table.mucno);
            entity.Add("clientuserId", table.clientuserId);
            entity.Add("no", table.no);
            entity.Add("nickname", table.nickname);
            entity.Add("AvatarStorageRecordId", table.avatarStorageRecordId);
            String totalPinyin = null;
            String fristPinyin = null;
            if (!string.IsNullOrEmpty(table.nickname)) {
                totalPinyin = PinyinHelper.getTotalPinyin(table.nickname);
                fristPinyin = PinyinHelper.getFristPinyin(table.nickname);
            }
            entity.Add("totalPinyin", totalPinyin);
            entity.Add("fristPinyin", fristPinyin);


            Dictionary<string, object> entityExist = new Dictionary<string, object>();
            entityExist.Add("mucId", table.mucId);
            entityExist.Add("mucno", table.mucno);
            entityExist.Add("clientuserId", table.clientuserId);
            entityExist.Add("no", table.no);
            if (this.isExist("muc_members", entityExist)) {
                SQLiteParameter[] param = new SQLiteParameter[] {
                    new SQLiteParameter("mucId",table.mucId),
                    new SQLiteParameter("mucno",table.mucno),
                    new SQLiteParameter("no",table.no),
                    new SQLiteParameter("clientuserId",table.clientuserId)
                };
                count = this._mgr.Update("muc_members", entity, "mucId=@mucId and mucno=@mucno and no=@no and clientuserId=@clientuserId ", param);
            } else {
                count = this._mgr.Save("muc_members", entity);

            }

            try {
                if (cgroup == null) {
                    MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                    cgroup = new GroupCacheEntity();
                    cgroup.table = mucTable;
                }

                GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                if (cmember == null) {
                    cmember = new GroupMemberCacheEntity();
                }

                cmember.name = table.nickname;

                VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                if (cvcard == null) {
                    cvcard = new VcardCacheEntity();
                    VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                    if (vcardTable == null) {
                        vcardTable = new VcardsTable();
                        vcardTable.clientuserId = table.clientuserId;
                        vcardTable.no = table.no;
                        vcardTable.nickname = table.nickname;
                        vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                        VcardsDao.getInstance().save(vcardTable);
                    } else {
                        //todo 是否要更新vcard
                    }
                    cvcard.table = vcardTable;
                } else {
                    //todo 是否需要更新vcard
                }
                cmember.vcard = cvcard;
                CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }



        } catch (Exception e) {
            Log.Error(typeof(MucMembersDao), e);
        }
        return count;

    }

    /// <summary>
    /// 通过clientuserId查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public MucMembersTable findByMucIdAndClientuserId(String mucId, String clientuserId) {
        MucMembersTable table = null;
        GroupCacheEntity cgroup = this.getCacheEntity(mucId);

        GroupMemberCacheEntity gmember = CacheHelper.getInstance().getGroupMember(mucId, clientuserId);

        if (cgroup != null && gmember != null) {
            table = new MucMembersTable();
            table.mucId = cgroup.table.mucId;
            table.mucno = cgroup.table.no;
            table.clientuserId = gmember.vcard.table.clientuserId;
            table.nickname = gmember.name;
            table.no = gmember.vcard.table.no;
            return table;
        } else {
            try {
                table = new MucMembersTable();
                Dictionary<string, object> entity = new Dictionary<string, object>();
                entity.Add("mucId", mucId);
                entity.Add("clientuserId", clientuserId);
                DataRow dataRow = this._mgr.QueryOne("muc_members", entity);
                if (dataRow == null) return null;
                table = DataUtils.DataTableToModel<MucMembersTable>(dataRow.Table);


                try {
                    if (cgroup == null) {
                        MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                        cgroup = new GroupCacheEntity();
                        cgroup.table = mucTable;
                    }

                    GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                    if (cmember == null) {
                        cmember = new GroupMemberCacheEntity();
                    }

                    cmember.name = table.nickname;

                    VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                    if (cvcard == null) {
                        cvcard = new VcardCacheEntity();
                        VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                        if (vcardTable == null) {
                            vcardTable = new VcardsTable();
                            vcardTable.clientuserId = table.clientuserId;
                            vcardTable.no = table.no;
                            vcardTable.nickname = table.nickname;
                            vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                            VcardsDao.getInstance().save(vcardTable);
                        } else {
                            //todo 是否要更新vcard
                        }
                        cvcard.table = vcardTable;
                    } else {
                        //todo 是否需要更新vcard
                    }
                    cmember.vcard = cvcard;
                    CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

                } catch (Exception e) {
                    Log.Error(typeof(MucMembersDao), e);
                }

            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }
            return table;
        }
    }

    /// <summary>
    /// 通过userNo查找
    /// </summary>
    /// <param Name="mucId"></param>
    /// <param Name="no"></param>
    /// <returns></returns>
    public MucMembersTable findByMucIdAndMemberNo(String mucId, String no) {
        MucMembersTable table = null;

        GroupCacheEntity cgroup = this.getCacheEntity(mucId);

        GroupMemberCacheEntity gmember = CacheHelper.getInstance().getGroupMember(mucId, no);

        if (cgroup != null && gmember != null) {
            table = new MucMembersTable();
            table.mucId = cgroup.table.mucId;
            table.mucno = cgroup.table.no;
            table.clientuserId = gmember.vcard.table.clientuserId;
            table.nickname = gmember.name;
            table.no = gmember.vcard.table.no;
            table.avatarStorageRecordId = gmember.vcard.table.avatarStorageRecordId;
            return table;
        } else {

            try {
                table = new MucMembersTable();
                Dictionary<string, object> entity = new Dictionary<string, object>();
                entity.Add("mucId", mucId);
                entity.Add("no", no);
                DataRow dataRow = this._mgr.QueryOne("muc_members", entity);
                if (dataRow == null) return null;
                table = DataUtils.DataTableToModel<MucMembersTable>(dataRow.Table);



                try {
                    if (cgroup == null) {
                        MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                        cgroup = new GroupCacheEntity();
                        cgroup.table = mucTable;
                    }

                    GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                    if (cmember == null) {
                        cmember = new GroupMemberCacheEntity();
                    }

                    cmember.name = table.nickname;

                    VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                    if (cvcard == null) {
                        cvcard = new VcardCacheEntity();
                        VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                        if (vcardTable == null) {
                            vcardTable = new VcardsTable();
                            vcardTable.clientuserId = table.clientuserId;
                            vcardTable.no = table.no;
                            vcardTable.nickname = table.nickname;
                            vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                            VcardsDao.getInstance().save(vcardTable);
                        } else {
                            //todo 是否要更新vcard
                        }
                        cvcard.table = vcardTable;
                    } else {
                        //todo 是否需要更新vcard
                    }
                    cmember.vcard = cvcard;
                    CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

                } catch (Exception e) {
                    Log.Error(typeof(MucMembersDao), e);
                }

            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }
            return table;
        }
    }


    /// <summary>
    /// 通过clientuserId查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public MucMembersTable findByMucNoAndClientuserId(String mucno, String clientuserId) {
        MucMembersTable table = null;

        GroupCacheEntity cgroup = this.getCacheEntity(mucno);

        GroupMemberCacheEntity gmember = CacheHelper.getInstance().getGroupMember(mucno, clientuserId);

        if (cgroup != null && gmember != null) {
            table = new MucMembersTable();
            table.mucId = cgroup.table.mucId;
            table.mucno = cgroup.table.no;
            table.clientuserId = gmember.vcard.table.clientuserId;
            table.nickname = gmember.name;
            table.no = gmember.vcard.table.no;
            return table;
        } else {

            try {
                table = new MucMembersTable();
                Dictionary<string, object> entity = new Dictionary<string, object>();
                entity.Add("mucno", mucno);
                entity.Add("clientuserId", clientuserId);
                DataRow dataRow = this._mgr.QueryOne("muc_members", entity);
                if (dataRow == null) return null;
                table = DataUtils.DataTableToModel<MucMembersTable>(dataRow.Table);


                try {
                    if (cgroup == null) {
                        MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                        cgroup = new GroupCacheEntity();
                        cgroup.table = mucTable;
                    }

                    GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                    if (cmember == null) {
                        cmember = new GroupMemberCacheEntity();
                    }

                    cmember.name = table.nickname;

                    VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                    if (cvcard == null) {
                        cvcard = new VcardCacheEntity();
                        VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                        if (vcardTable == null) {
                            vcardTable = new VcardsTable();
                            vcardTable.clientuserId = table.clientuserId;
                            vcardTable.no = table.no;
                            vcardTable.nickname = table.nickname;
                            vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                            VcardsDao.getInstance().save(vcardTable);
                        } else {
                            //todo 是否要更新vcard
                        }
                        cvcard.table = vcardTable;
                    } else {
                        //todo 是否需要更新vcard
                    }
                    cmember.vcard = cvcard;
                    CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

                } catch (Exception e) {
                    Log.Error(typeof(MucMembersDao), e);
                }


            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }
            return table;
        }
    }

    /// <summary>
    /// 通过userNo查找
    /// </summary>
    /// <param Name="mucId"></param>
    /// <param Name="no"></param>
    /// <returns></returns>
    public MucMembersTable findByMucNoAndMemberNo(String mucno, String no) {
        MucMembersTable table = null;

        GroupCacheEntity cgroup = this.getCacheEntity(mucno);

        GroupMemberCacheEntity gmember = CacheHelper.getInstance().getGroupMember(mucno, no);

        if (cgroup != null && gmember != null ) {
            table = new MucMembersTable();
            table.mucId = cgroup.table.mucId;
            table.mucno = cgroup.table.no;
            table.nickname = gmember.name;
            table.clientuserId = gmember.vcard.table.clientuserId;
            table.no = gmember.vcard.table.no;
            return table;
        } else {


            try {
                table = new MucMembersTable();
                Dictionary<string, object> entity = new Dictionary<string, object>();
                entity.Add("mucno", mucno);
                entity.Add("no", no);
                DataRow dataRow = this._mgr.QueryOne("muc_members", entity);
                if (dataRow == null) return null;
                table = DataUtils.DataTableToModel<MucMembersTable>(dataRow.Table);

                try {
                    if (cgroup == null) {
                        MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                        cgroup = new GroupCacheEntity();
                        cgroup.table = mucTable;
                    }

                    GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                    if (cmember == null) {
                        cmember = new GroupMemberCacheEntity();
                    }

                    cmember.name = table.nickname;

                    VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                    if (cvcard == null) {
                        cvcard = new VcardCacheEntity();
                        VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                        if (vcardTable == null) {
                            vcardTable = new VcardsTable();
                            vcardTable.clientuserId = table.clientuserId;
                            vcardTable.no = table.no;
                            vcardTable.nickname = table.nickname;
                            vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                            VcardsDao.getInstance().save(vcardTable);
                        } else {
                            //todo 是否要更新vcard
                        }
                        cvcard.table = vcardTable;
                    } else {
                        //todo 是否需要更新vcard
                    }
                    cmember.vcard = cvcard;
                    CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

                } catch (Exception e) {
                    Log.Error(typeof(MucMembersDao), e);
                }
            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }
            return table;
        }
    }




    /// <summary>
    /// 通过群NO删除
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public int deleteByMucNo(String mucno) {
        int count = this._mgr.Delete("muc_members", "mucno=@mucno", new SQLiteParameter[] {
            new SQLiteParameter("mucno", mucno)
        });
        return count;

    }

    /// <summary>
    /// 通过clientuserId删除
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public int deleteByClientuserId(String mucId, String clientuserId) {

        GroupCacheEntity cgroup = this.getCacheEntity(mucId);
        if (cgroup != null) {
            CacheHelper.getInstance().removeGroupMember(mucId, clientuserId);
        }

        Dictionary<string, object> entity = new Dictionary<string, object>();
        entity.Add("mucId", mucId);
        entity.Add("clientuserId", clientuserId);
        int count = this._mgr.Delete("muc_members", entity);



        return count;

    }




    /// <summary>
    /// 通过群NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<MucMembersTable> findByMucNo(String mucno) {

        MucTable mucTable= MucDao.getInstance().FindGroupByNo(mucno);
        GroupCacheEntity cgroup =  this.getCacheEntity(mucno);

        if (cgroup != null && cgroup.getMembers() != null && cgroup.getMembers().Count > 0 && mucTable.count== cgroup.getMembers().Count) {
            IList members = cgroup.getMembers();

            List<MucMembersTable> rtlist = new List<MucMembersTable>();
            foreach (object obj in members) {
                GroupMemberCacheEntity entity = (GroupMemberCacheEntity)obj;
                VcardsTable vcard = entity.vcard.table;
                MucMembersTable mucMember = new MucMembersTable();
                mucMember.mucId = cgroup.table.mucId;
                mucMember.mucno = cgroup.table.no;
                mucMember.clientuserId = vcard.clientuserId;
                mucMember.no = vcard.no;
                mucMember.avatarStorageRecordId = vcard.avatarStorageRecordId;
                if (string.IsNullOrEmpty(entity.name)) {
                    mucMember.nickname = vcard.nickname;
                } else {
                    mucMember.nickname = entity.name;
                }
                mucMember.updateTime = vcard.updateTime;
                rtlist.Add(mucMember);
            }
            return rtlist;
        }

        List<MucMembersTable> list = null;
        try {
            //DataRow entity = this._mgr.QueryOne("vcards", "mucno", no);

            list = new List<MucMembersTable>();
            DataTable dtTable = this._mgr.QueryDt("muc_members", "mucno", mucno);
            list = DataUtils.DataTableToModelList<MucMembersTable>(dtTable);

            foreach (MucMembersTable table in list) {

                try {
                    if (cgroup == null) {
                        //MucTable mucTable = MucDao.getInstance().FindGroupByNo(table.mucno);
                        cgroup = new GroupCacheEntity();
                        cgroup.table = mucTable;
                    }

                    GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(table.mucno, table.no);

                    if (cmember == null) {
                        cmember = new GroupMemberCacheEntity();
                    }

                    cmember.name = table.nickname;

                    VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(table.no);
                    if (cvcard == null) {
                        cvcard = new VcardCacheEntity();
                        VcardsTable vcardTable = VcardsDao.getInstance().findByNo(table.no);
                        if (vcardTable == null) {
                            vcardTable = new VcardsTable();
                            vcardTable.clientuserId = table.clientuserId;
                            vcardTable.no = table.no;
                            vcardTable.nickname = table.nickname;
                            vcardTable.avatarStorageRecordId = table.avatarStorageRecordId;
                            VcardsDao.getInstance().save(vcardTable);
                        } else {
                            //todo 是否要更新vcard
                        }
                        cvcard.table = vcardTable;
                    } else {
                        //todo 是否需要更新vcard
                    }
                    cmember.vcard = cvcard;
                    CacheHelper.getInstance().addGroupMember(table.mucno, cmember);

                } catch (Exception e) {
                    Log.Error(typeof(MucMembersDao), e);
                }

            }


        } catch (Exception e) {
            Log.Error(typeof(MucMembersDao), e);
        }
        return list;

    }


    /// <summary>
    /// 通过群NO查找
    /// </summary>
    /// <param Name="no"></param>
    /// <returns></returns>
    public List<MucMembersTable> findByMucNoFromDb(String mucno) {

        List<MucMembersTable> list = null;
        try {
            //DataRow entity = this._mgr.QueryOne("vcards", "mucno", no);

            list = new List<MucMembersTable>();
            DataTable dtTable = this._mgr.QueryDt("muc_members", "mucno", mucno);
            list = DataUtils.DataTableToModelList<MucMembersTable>(dtTable);
        } catch (Exception e) {
            Log.Error(typeof(MucMembersDao), e);
        }
        return list;

    }

    /// <summary>
    /// 根据NO修改某个人在所有群的头像
    /// </summary>
    /// <param Name="memberNo"></param>
    /// <param Name="avatarStorageId"></param>
    /// <returns></returns>
    public int saveMucMemberAvatarByNo(String memberNo,String avatarStorageId) {
        int count = -1;

        try {
            String sql = "update muc_members set AvatarStorageRecordId = '" + avatarStorageId
                         + "' where no = '" + memberNo + "'";
            count = this._mgr.ExecuteNonQuery(sql,null);

            VcardCacheEntity vcard = CacheHelper.getInstance().getVcard(memberNo);
            if(vcard != null && vcard.table != null)
                vcard.table.avatarStorageRecordId = avatarStorageId;

        } catch (Exception e) {
            Log.Error(typeof(MucMembersDao), e);
        }
        return count;

    }

    /// <summary>
    /// 插入群成员
    /// </summary>
    /// <param Name="memberList"></param>
    /// <param Name="no"></param>
    /// <param Name="mucId"></param>
    public void InsertGroupMember(IList<MucMembersTable> memberList, string no, string mucId) {

        _mgr.Delete("muc_members", "Mucno=@Mucno", new SQLiteParameter[] {
            new SQLiteParameter("Mucno", no)
        });

        GroupCacheEntity cgroup = this.getCacheEntity(no);



        List<Dictionary<string, object>> entitys = new List<Dictionary<string, object>>();
        for (int i = 0; i < memberList.Count; i++) {
            MucMembersTable bean = memberList[i];
            bean.mucno = no;
            bean.mucId = mucId;
            this.save(bean);
            //Dictionary<string, object> entity = new Dictionary<string, object>();

            //entity.Add("mucno", no);
            //entity.Add("mucid", mucId);
            //entity.Add("no", bean.no);
            //entity.Add("clientuserid", bean.clientuserId);
            //entity.Add("nickname", bean.nickname);
            //entity.Add("AvatarStorageRecordId", bean.AvatarStorageRecordId);
            //entitys.Add(entity);
            try {
                if (cgroup == null) {
                    MucTable mucTable = MucDao.getInstance().FindGroupByNo(no);
                    cgroup = new GroupCacheEntity();
                    cgroup.table = mucTable;
                }

                GroupMemberCacheEntity cmember = CacheHelper.getInstance().getGroupMember(no, bean.no);

                if (cmember == null) {
                    cmember = new GroupMemberCacheEntity();
                }

                cmember.name = bean.nickname;

                VcardCacheEntity cvcard = CacheHelper.getInstance().getVcard(bean.no);
                if (cvcard == null) {
                    cvcard = new VcardCacheEntity();
                    VcardsTable vcardTable = VcardsDao.getInstance().findByNo(bean.no);
                    if (vcardTable == null) {
                        vcardTable = new VcardsTable();
                        vcardTable.clientuserId = bean.clientuserId;
                        vcardTable.no = bean.no;
                        vcardTable.nickname = bean.nickname;
                        vcardTable.avatarStorageRecordId = bean.avatarStorageRecordId;
                        VcardsDao.getInstance().save(vcardTable);
                    } else {
                        //todo 是否要更新vcard
                    }
                    cvcard.table = vcardTable;
                } else {
                    //todo 是否需要更新vcard
                }
                cmember.vcard = cvcard;
                CacheHelper.getInstance().addGroupMember(no, cmember);

            } catch (Exception e) {
                Log.Error(typeof(MucMembersDao), e);
            }

        }
        //this._mgr.Save("muc_members", entitys);

        //_mgr.CommitTrans();

    }

    /// <summary>
    /// 重构没有拼音的数据
    /// </summary>
    public void ReBuildDataWithoutPinyin() {

        try {
            List<MucMembersTable> list = null;
            String sql = "select * from muc_members where totalPinyin is null or totalPinyin = ''";
            DataTable dt = this._mgr.ExecuteRow(sql, null);
            list = DataUtils.DataTableToModelList<MucMembersTable>(dt);
            if (list != null) {
                foreach (MucMembersTable table in list) {
                    this.save(table);
                }
            }
        } catch (Exception e) {
            Log.Error(typeof(MucMembersDao), e);
        }
    }
}
}
