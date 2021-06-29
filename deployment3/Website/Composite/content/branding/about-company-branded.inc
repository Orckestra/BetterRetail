<ui:dialogpage height="380" width="380" resizable="false" branded="true">
	<div class="dailog-about-content">
		<p>
			Commerce Cloud Version <span id="version"></span>
		</p>
		<p>
			Warning: This computer program is protected by copyright laws and international treaties.
			Unauthorized reproduction or redistribution of this program, 
			or any portion of it may result in severe civil and criminal penalties, 
			and will be prosecuted to the maximum extent possible under the law.
			Orckestra and Orckestra Commerce Cloud are trademarks of Orckestra Technologies Inc.
			All other trademarks are property of the respective owners.
		</p>
		<p class="text-primary">
			Orckestra Commerce Cloud
		</p>
		<p id="copyright">
			<span id="year"></span> Orckestra.
			<br/>
			All rights reserved.
		</p>
	</div>
	<script type="text/javascript">
		var version = document.getElementById("version");
		var year = document.getElementById("year");
		version.innerHTML = Installation.versionString;
		year.innerHTML = new Date().getFullYear();
	</script>
</ui:dialogpage>