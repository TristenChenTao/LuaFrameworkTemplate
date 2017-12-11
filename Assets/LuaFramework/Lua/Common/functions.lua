-- 系统默认约定的名字，忽略掉不创建对应引用，否则会异常
function isNotSystemName(ctrlName)
	if ctrlName == "button" or ctrlName == "title" or ctrlName == "icon" or ctrlName == "list" or ctrlName == "bar" or ctrlName == "bar_v" or ctrlName == "ani"
		or ctrlName == "grip" or ctrlName == "arrow1" or ctrlName == "arrow2" or ctrlName == "frame" then
		return false
	else
		return true
	end 
end

-- 自动为GComponent组件创建子控件对应的lua table同名域的对象引用
function AutoCreateCompChild(contentPane, luatb)
	local objName, childobj
	for i = 0, contentPane.numChildren - 1 do
		childobj = contentPane:GetChildAt(i)
		objName = childobj.name
		if (string.byte(objName, 1) == 110) and (tonumber(string.sub(objName, 2)) ~= nil) then
		else if isNotSystemName(objName) then
				 print("auto create child prop : " .. objName)
				luatb[objName] = childobj
			end
		end
	end

end

-- @function: 打印table的内容，递归
-- @param: tbl 要打印的table
-- @param: level 递归的层数，默认不用传值进来
-- @param: filteDefault 是否过滤打印构造函数，默认为是
-- @return: return
function printTable( tbl , level, filteDefault)
  local msg = ""
  filteDefault = filteDefault or true --默认过滤关键字（DeleteMe, _class_type）
  level = level or 1
  local indent_str = ""
  for i = 1, level do
    indent_str = indent_str.."  "
  end

  print(indent_str .. "{")
  for k,v in pairs(tbl) do
    if filteDefault then
      if k ~= "_class_type" and k ~= "DeleteMe" then
        local item_str = string.format("%s%s = %s", indent_str .. " ",tostring(k), tostring(v))
        print(item_str)
        if type(v) == "table" then
          printTable(v, level + 1)
        end
      end
    else
      local item_str = string.format("%s%s = %s", indent_str .. " ",tostring(k), tostring(v))
      print(item_str)
      if type(v) == "table" then
        printTable(v, level + 1)
      end
    end
  end
  print(indent_str .. "}")
end

--输出日志--
function log(str)
    Util.Log(str);
end

--错误日志--
function logError(str) 
	Util.LogError(str);
end

--警告日志--
function logWarn(str) 
	Util.LogWarning(str);
end

--查找对象--
function find(str)
	return GameObject.Find(str);
end

function destroy(obj)
	GameObject.Destroy(obj);
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

--创建面板--
function createPanel(name)
	PanelManager:CreatePanel(name);
end

function child(str)
	return transform:FindChild(str);
end

function subGet(childNode, typeName)		
	return child(childNode):GetComponent(typeName);
end

function findPanel(str) 
	local obj = find(str);
	if obj == nil then
		error(str.." is null");
		return nil;
	end
	return obj:GetComponent("BaseLua");
end

function file_exists(name)
   local f=io.open(name,"r")
   if f~=nil then io.close(f) return true else return false end
end

local function exportstring( s )
	return string.format("%q", s)
end

--// The Save Function
function table.save(tbl,filename)
	local charS,charE = "   ","\n"
	local file,err = io.open( filename, "wb" )
	if err then return err end

	-- initiate variables for save procedure
	local tables,lookup = { tbl },{ [tbl] = 1 }
	file:write( "return {"..charE )

	for idx,t in ipairs( tables ) do
		file:write( "-- Table: {"..idx.."}"..charE )
		file:write( "{"..charE )
		local thandled = {}

		for i,v in ipairs( t ) do
			thandled[i] = true
			local stype = type( v )
			-- only handle value
			if stype == "table" then
				if not lookup[v] then
					table.insert( tables, v )
					lookup[v] = #tables
				end
				file:write( charS.."{"..lookup[v].."},"..charE )
			elseif stype == "string" then
				file:write(  charS..exportstring( v )..","..charE )
			elseif stype == "number" then
				file:write(  charS..tostring( v )..","..charE )
			end
		end

		for i,v in pairs( t ) do
			-- escape handled values
			if (not thandled[i]) then

				local str = ""
				local stype = type( i )
				-- handle index
				if stype == "table" then
					if not lookup[i] then
						table.insert( tables,i )
						lookup[i] = #tables
					end
					str = charS.."[{"..lookup[i].."}]="
				elseif stype == "string" then
					str = charS.."["..exportstring( i ).."]="
					
				elseif stype == "number" then
					str = charS.."["..tostring( i ).."]="
				end

				if str ~= "" then
					stype = type( v )
					-- handle value
					if stype == "table" then
						if not lookup[v] then
							table.insert( tables,v )
							lookup[v] = #tables
						end
						file:write( str.."{"..lookup[v].."},"..charE )
					elseif stype == "string" then
						file:write( str..exportstring( v )..","..charE )
					elseif stype == "number" then
						file:write( str..tostring( v )..","..charE )
					elseif stype == "boolean" then
						file:write( str..tostring( v )..","..charE )
					end
				end
			end
		end
		file:write( "},"..charE )
	end
	file:write( "}" )
	file:close()
end

--// The Load Function
function table.load(sfile)
	local ftables,err = loadfile( sfile )
	if err then return _,err end
	local tables = ftables()
	if not tables then
		return false;
	end
	for idx = 1,#tables do
		local tolinki = {}
		for i,v in pairs( tables[idx] ) do
			if type( v ) == "table" then
				tables[idx][i] = tables[v[1]]
			end
			if type( i ) == "table" and tables[i[1]] then
				table.insert( tolinki,{ i,tables[i[1]] } )
			end
		end
		-- link indices
		for _,v in ipairs( tolinki ) do
			tables[idx][v[2]],tables[idx][v[1]] =  tables[idx][v[1]],nil
		end
	end
	return tables[1]
end

-- 此方法用于字符串输出一个table对象的全部内容
table.tostring = function(t)
	local mark={}
	local assign={}
	local function ser_table(tbl,parent, indent)
		mark[tbl]=parent
		local tmp={}
		for k,v in pairs(tbl) do
			local key= type(k)=="number" and "["..k.."]" or "[".. string.format("%q", k) .."]"
			if type(v)=="table" then
				local dotkey= parent.. key
				if mark[v] then
					table.insert(assign,dotkey.."='"..mark[v] .."'\n" .. string.rep("	", indent))
				else
					table.insert(tmp, key.."="..ser_table(v,dotkey, indent + 1) .. "\n" .. string.rep("	", indent))
				end
			elseif type(v) == "string" then
				table.insert(tmp, key.."=".. string.format('%q', v) .. "\n" .. string.rep("	", indent))
			elseif type(v) == "number" or type(v) == "boolean" then
				table.insert(tmp, key.."=".. tostring(v) .. "\n" .. string.rep("	", indent))
			elseif type(v) == "userdata" then
				table.insert(tmp, key.."=".. "userData" .. "\n" .. string.rep("	", indent))
			end
	   end
	   return "{\n" .. string.rep("	", indent)..table.concat(tmp,",").."}"
	end
	return "do local ret="..ser_table(t,"ret",0)..table.concat(assign," ").." return ret end"
end
